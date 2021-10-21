/******************************************************************************
 * Copyright (C) Ultraleap, Inc. 2011-2020.                                   *
 *                                                                            *
 * Use subject to the terms of the Apache License 2.0 available at            *
 * http://www.apache.org/licenses/LICENSE-2.0, or another agreement           *
 * between Ultraleap and you, your company or other organization.             *
 ******************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Leap.Unity.HandsModule {
    public static class HandBinderAutoBinder {

        //The minimum amount of Transforms required to be able to match Transforms back to a Leap data point
        const int MINIMUM_TRANSFORMS = 3;

        /// <summary>
        /// This function is used to search the child Transforms of the HandBinder script to automatically try and assign them for the user
        /// </summary>
        /// <param name="handBinder">The HandBinder that the found Transform target will get assigned to</param>
        public static void AutoBind(HandBinder handBinder) {

            handBinder.ResetHand();
            BoneNameDefinitions boneDefinitions = new BoneNameDefinitions();

            //Get all children of the hand
            var children = new List<Transform>();
            children.Add(handBinder.transform);
            children.AddRange(GetAllChildren(handBinder.transform));
            
            var thumbBones = SortBones(SelectBones(children, boneDefinitions.DefinitionThumb), false, true);
            var indexBones = SortBones(SelectBones(children, boneDefinitions.DefinitionIndex), handBinder.UseMetaBones);
            var middleBones = SortBones(SelectBones(children, boneDefinitions.DefinitionMiddle), handBinder.UseMetaBones);
            var ringBones = SortBones(SelectBones(children, boneDefinitions.DefinitionRing), handBinder.UseMetaBones);
            var pinkyBones = SortBones(SelectBones(children, boneDefinitions.DefinitionPinky), handBinder.UseMetaBones);
            var wrist = SelectBones(children, boneDefinitions.DefinitionWrist).FirstOrDefault();
            var elbow = SelectBones(children, boneDefinitions.DefinitionElbow).FirstOrDefault();

            handBinder.BoundHand.fingers[0].boundBones = AssignTransformToBoundBone(thumbBones);
            handBinder.BoundHand.fingers[1].boundBones = AssignTransformToBoundBone(indexBones);
            handBinder.BoundHand.fingers[2].boundBones = AssignTransformToBoundBone(middleBones);
            handBinder.BoundHand.fingers[3].boundBones = AssignTransformToBoundBone(ringBones);
            handBinder.BoundHand.fingers[4].boundBones = AssignTransformToBoundBone(pinkyBones);
            handBinder.BoundHand.wrist = AssignBoundBone(wrist);
            handBinder.BoundHand.elbow = AssignBoundBone(elbow);

            if(wrist != null && elbow != null) {
                handBinder.ElbowLength = (wrist.position - elbow.position).magnitude;
            }

            EstimateWristRotationOffset(handBinder);
            CalculateElbowLength(handBinder);

            handBinder.GetLeapHand();
            handBinder.UpdateHand();
            handBinder.DebugModelTransforms = true;
            handBinder.SetEditorPose = true;
        }

        /// <summary>
        /// Get all the children of a transform
        /// </summary>
        /// <param name="_t"></param>
        /// <returns></returns>
        public static List<Transform> GetAllChildren(Transform _t) {
            List<Transform> ts = new List<Transform>();
            foreach(Transform t in _t) {
                ts.Add(t);
                if(t.childCount > 0) {
                    ts.AddRange(GetAllChildren(t));
                }
            }
            return ts;
        }

        /// <summary>
        /// The Autobinder uses this to select the children that match the finger definitions
        /// </summary>
        /// <param name="children">The found children</param>
        /// <param name="definitions">The criteria to match to the children</param>
        /// <returns></returns>
        private static Transform[] SelectBones(List<Transform> children, string[] definitions) {
            var bones = new List<Transform>();
            for(int definitionIndex = 0; definitionIndex < definitions.Length; definitionIndex++) {
                foreach(var child in children) {
                    //We have found all the bones we need
                    if(bones.Count == 4) {
                        break;
                    }

                    var definition = definitions[definitionIndex].ToUpper();

                    if(child.name.ToUpper().Contains(definition)) {
                        bones.Add(child);
                    }
                }
            }
            return bones.ToArray();
        }

        /// <summary>
        /// Sort through the bones to identify which BoneType they all belong to
        /// </summary>
        /// <param name="bones">The bones to sort</param>
        /// <param name="isThumb">Is it a thumb</param>
        /// <returns></returns>
        private static Transform[] SortBones(Transform[] bones, bool useMeta = false, bool isThumb = false) {
            Transform meta = null;
            Transform proximal = null;
            Transform middle = null;
            Transform distal = null;

            if(bones.Length == MINIMUM_TRANSFORMS) {

                if(useMeta == false) {
                    meta = null;
                    proximal = bones[0];
                    middle = bones[1];
                    distal = bones[2];
                }
                else {
                    meta = bones[0];
                    proximal = bones[1];
                    middle = bones[2];
                    distal = null;
                }
            }
            else if(bones.Length > MINIMUM_TRANSFORMS) {

                if(isThumb == true) {
                    proximal = bones[0];
                    middle = bones[1];
                    distal = bones[2];
                }
                else {
                    meta = bones[0];
                    proximal = bones[1];
                    middle = bones[2];
                    distal = bones[3];
                }
            }

            var boundObjects = new Transform[]
            {
                meta,
                proximal,
                middle,
                distal
            };

            return boundObjects;
        }

        /// <summary>
        /// Bind a transform in the scene to the Hand Binder
        /// </summary>
        /// <param name="boneTransform">The transform you want to assign </param>
        /// <param name="fingerIndex"> The index of the finger you want to assign</param>
        /// <param name="boneIndex">The index of the bone you want to assign</param>
        /// <param name="handBinder">The Hand Binder this information will be added to</param>
        /// <returns></returns>
        public static BoundBone[] AssignTransformToBoundBone(Transform[] boneTransform) {
            var boundFingers = new BoundBone[]
                {
                    AssignBoundBone(boneTransform[0]),
                    AssignBoundBone(boneTransform[1]),
                    AssignBoundBone(boneTransform[2]),
                    AssignBoundBone(boneTransform[3]),
                };

            return boundFingers;
        }

        public static BoundBone AssignBoundBone(Transform transform) {
            var newBone = new BoundBone();
            if(transform != null) {
                newBone.boundTransform = transform;
                newBone.startTransform = new TransformStore();
                newBone.startTransform.position = transform.localPosition;
                newBone.startTransform.rotation = transform.localRotation.eulerAngles;
            }
            return newBone;
        }

        /// <summary>
        /// Estimate the rotation offset needed to get the rigged hand into the same orientation as the leap hand
        /// </summary>
        public static void EstimateWristRotationOffset(HandBinder handBinder) {

            Transform indexBone = handBinder.BoundHand.fingers[(int)Finger.FingerType.TYPE_INDEX].boundBones[(int)Bone.BoneType.TYPE_PROXIMAL].boundTransform;
            Transform middleBone = handBinder.BoundHand.fingers[(int)Finger.FingerType.TYPE_MIDDLE].boundBones[(int)Bone.BoneType.TYPE_PROXIMAL].boundTransform;
            Transform pinkyBone = handBinder.BoundHand.fingers[(int)Finger.FingerType.TYPE_PINKY].boundBones[(int)Bone.BoneType.TYPE_PROXIMAL].boundTransform;

            Transform wrist = handBinder.BoundHand.wrist.boundTransform;

            if(middleBone != null && indexBone != null && pinkyBone != null && wrist != null) {

                //Calculate model's rotation
                var forward = (middleBone.position - wrist.position);
                var right = (indexBone.position - pinkyBone.position);
                if(handBinder.Handedness == Chirality.Right) {
                    right = -right;
                }
                var up = Vector3.Cross(forward, right);
                Vector3.OrthoNormalize(ref up, ref forward, ref right);
                var modelRotation = Quaternion.LookRotation(forward, up);

                //Calculate the difference between the Calculated hand basis and the wrists rotation
                handBinder.WristRotationOffset = (Quaternion.Inverse(modelRotation) * wrist.transform.rotation).eulerAngles;
                //Assuming the fingers have been created using the same rotation axis as the wrist
                handBinder.GlobalFingerRotationOffset = handBinder.WristRotationOffset;
            }
        }

        public static void CalculateElbowLength(HandBinder handBinder) {
            if(handBinder.BoundHand.elbow.boundTransform != null && handBinder.BoundHand.wrist.boundTransform != null) {
                handBinder.ElbowLength = (handBinder.BoundHand.wrist.boundTransform.position - handBinder.BoundHand.elbow.boundTransform.position).magnitude;
            }
        }
    }
}

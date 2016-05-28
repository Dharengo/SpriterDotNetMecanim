using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System;
using System.Collections.Generic;

namespace SpriterDotNetMecanim.Editor {
	using Editor = UnityEditor.Editor;
	[CustomEditor (typeof (AnimatorRelay))] public class RelayEditor : Editor {
		protected new AnimatorRelay target;
		private AnimatorState state;

		protected virtual void CheckCorrectUse () {
			var context = AnimatorController.FindStateMachineBehaviourContext (target) [0];
			state = context.animatorObject as AnimatorState;
			if (!state) Debug.LogError ("AnimatorRelay will not work correctly if not attached to an AnimatorState");
		}

		protected virtual AnimatorStateTransition[] GetTransitions () {
			return state.transitions;
		}

		private void OnEnable () {
			target = (AnimatorRelay)base.target;
			if (target.CustomTransitions == null) target.CustomTransitions = new AnimatorRelay.TransitionList ();
			CheckCorrectUse ();
			var toRemove = new List<int> ();
			foreach (AnimatorRelay.TransitionList.CustomTransition transition in target.CustomTransitions) {
				var trans = ArrayUtility.Find (GetTransitions (), x => Match (x, transition.TargetID));
				if (!trans) toRemove.Add (transition.TargetID);
			}
			foreach (var id in toRemove) target.CustomTransitions [id] = -1f;
		}

		private static bool Match (AnimatorStateTransition transition, int stateID) {
			var state = transition.destinationState;
			if (state) return state.nameHash == stateID;
			else return false;
		}

		public override void OnInspectorGUI () {
			DrawDefaultInspector ();
			var transitions = GetTransitions ();
			if (transitions.Length <= 0) return;
			if (target.CustomTransitions.Count <= 0)
				EditorGUILayout.LabelField ("No custom transitions have been set.");
			else EditorGUILayout.LabelField ("Custom transition durations (in milliseconds):");
			foreach (AnimatorRelay.TransitionList.CustomTransition transition in target.CustomTransitions) {
				EditorGUILayout.BeginHorizontal ();
				EditorGUI.BeginChangeCheck ();
				var newID = Transitions (transition.TargetID);
				if (EditorGUI.EndChangeCheck () && newID != transition.TargetID) {
					target.CustomTransitions [transition.TargetID] = -1f;
					target.CustomTransitions [newID] = transition.Duration;
					break;
				}
				EditorGUI.BeginChangeCheck ();
				var newDuration = EditorGUILayout.FloatField (transition.Duration);
				if (EditorGUI.EndChangeCheck () && newDuration != transition.Duration)
					target.CustomTransitions [transition.TargetID] = newDuration >= 0f ? newDuration : 0f;
				if (GUILayout.Button ("x")) {
					target.CustomTransitions [transition.TargetID] = -1f;
					break;
				}
				EditorGUILayout.EndHorizontal ();
			}
			if (target.CustomTransitions.Count < transitions.Length && GUILayout.Button ("New custom transition")) {
				foreach (var transition in transitions) {
					var newState = transition.destinationState;
					if (newState && !target.CustomTransitions.Contains (newState.nameHash)) {
						target.CustomTransitions [newState.nameHash] = 500f;
						break;
					}
				}
			}
			if (GUI.changed) EditorUtility.SetDirty (target);
		}

		private int Transitions (int currentVal) {
			var transitions = new List<string> ();
			var ids = new List<int> ();
			foreach (var transition in GetTransitions ()) {
				var newState = transition.destinationState;
				if (newState) {
					var id = newState.nameHash;
					if (currentVal != id && target.CustomTransitions.Contains (id)) continue;
					transitions.Add (newState.name);
					ids.Add (id);
				}
			}
			return EditorGUILayout.IntPopup (currentVal, transitions.ToArray (), ids.ToArray ());
		}
	}

	[CustomEditor (typeof (AnyStateRelay))] public class AnyStateEditor : RelayEditor {
		private AnimatorStateMachine machine;

		protected override void CheckCorrectUse () {
			var context = AnimatorController.FindStateMachineBehaviourContext (target) [0];
			machine = context.animatorObject as AnimatorStateMachine;
			if (!machine || !IsRootMachine (machine, context.animatorController, context.layerIndex)) 
				Debug.LogError ("AnyStateRelay will not work correctly if not attached to the root AnimatorState");
		}

		private static bool IsRootMachine (AnimatorStateMachine machine, AnimatorController controller, int layer) {
			return machine == controller.layers [layer].stateMachine;
		}

		protected override AnimatorStateTransition[] GetTransitions () {
			return machine.anyStateTransitions;
		}
	}
}

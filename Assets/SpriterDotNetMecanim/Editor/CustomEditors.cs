using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System;
using System.Collections.Generic;

namespace SpriterDotNetMecanim.Editor {
	using Editor = UnityEditor.Editor;
	[CustomEditor (typeof (AnimatorRelay))] public class RelayEditor : Editor {
		private new AnimatorRelay target;
		private AnimatorState state;

		private void OnEnable () {
			target = (AnimatorRelay)base.target;
			var context = AnimatorController.FindStateMachineBehaviourContext (target) [0];
			state = context.animatorObject as AnimatorState;
			if (!state) Debug.LogError ("AnimatorRelay will not work correctly if not attached to an AnimatorState");
			if (target.CustomTransitions == null) target.CustomTransitions = new AnimatorRelay.TransitionList ();
			var toRemove = new List<int> ();
			foreach (var transition in target.CustomTransitions) {
				var trans = ArrayUtility.Find (state.transitions, x => Match (x, transition.TargetID));
				if (!trans) toRemove.Add (transition.TargetID);
			}
			foreach (var id in toRemove) target.CustomTransitions [id] = -1f;
			target.testRelay = target;
		}

		private static bool Match (AnimatorStateTransition transition, int stateID) {
			var state = transition.destinationState;
			if (state) return state.nameHash == stateID;
			else return false;
		}

		public override void OnInspectorGUI () {
			DrawDefaultInspector ();
			if (state.transitions.Length <= 0) return;
			if (target.CustomTransitions.Count <= 0)
				EditorGUILayout.LabelField ("No custom transitions have been set.");
			else EditorGUILayout.LabelField ("Custom transition durations (in milliseconds):");
			foreach (var transition in target.CustomTransitions) {
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
			if (target.CustomTransitions.Count < state.transitions.Length && GUILayout.Button ("New custom transition")) {
				foreach (var transition in state.transitions) {
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
			foreach (var transition in state.transitions) {
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
}

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SpriterDotNetMecanim {
	/// <summary>
	/// Sends messages from the AnimatorController to the AnimatorAssist component.
	/// </summary>
	public class AnimatorRelay : StateMachineBehaviour {
		/// <summary>
		/// Entries for transitions with custom durations
		/// </summary>
		[HideInInspector] public TransitionList CustomTransitions;

		/// <summary>
		/// Not meant to be called manually.
		/// </summary>
		public override void OnStateEnter (Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			var assistant = animator.GetComponent<AnimatorAssist> ();
			if (assistant) assistant.Transition (this, stateInfo.shortNameHash, layerIndex);
		}

		/// <summary>
		/// Poverty dictionary. It's poverty, but Unity can serialize it. The list shouldn't get long enough for it to matter anyway.
		/// </summary>
		[Serializable] public class TransitionList : IEnumerable<TransitionList.CustomTransition> {
			[SerializeField] private List<int> TargetIDS = new List<int> ();
			[SerializeField] private List<float> Durations = new List<float> ();

			public int Count { get { return TargetIDS.Count; } }

			public bool Contains (int targetID) {
				return TargetIDS.Contains (targetID);
			}

			public float this [int TargetID] {
				get {
					for (var i = 0; i < Count; i++)
						if (TargetID == TargetIDS [i])
							return Durations [i];
					return -1f;
				}
				set {
					for (var i = 0; i < Count; i++)
						if (TargetID == TargetIDS [i]) {
							if (value < 0) {
								TargetIDS.RemoveAt (i);
								Durations.RemoveAt (i);
								return;
							}
							Durations [i] = value;
							return;
						}
					TargetIDS.Add (TargetID);
					Durations.Add (value);
				}
			}

			IEnumerator<CustomTransition> IEnumerable<CustomTransition>.GetEnumerator () {
				for (var i = 0; i < Count; i++) yield return new CustomTransition (TargetIDS [i], Durations [i]);
			}

			IEnumerator IEnumerable.GetEnumerator () {
				return ((IEnumerable<CustomTransition>)this).GetEnumerator ();
			}

			public struct CustomTransition {
				public int TargetID;
				public float Duration;

				public CustomTransition (int id, float dur) {
					TargetID = id;
					Duration = dur;
				}
			}
		}
	}
}

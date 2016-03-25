using UnityEngine;
using SpriterDotNetMecanim;

public class AnyStateRelay : AnimatorRelay {
	public override void OnStateEnter (Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		var assistant = animator.GetComponent<AnimatorAssist> ();
		if (assistant) assistant.RegisterAnyState (this, layerIndex);
	}
}

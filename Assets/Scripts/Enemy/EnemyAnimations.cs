using UnityEngine;

public class EnemyAnimations : MonoBehaviour
{
    [SerializeField] private Animator animator;

    void Start() => animator = GetComponent<Animator>();

    public void PlayMoveAnim(bool state) => animator.SetBool("IsMoving", state);

    public void PlayAttackAnim() => animator.SetTrigger("Attack");

    public void PlayDeathAnim(bool isHealthAvail)
    {
        animator.SetTrigger("Death");
        if (isHealthAvail)
        {
            PlayRecoverAnim();
        }
    }

    public void PlayHurtAnim() => animator.SetTrigger("Hurt");

    private void PlayRecoverAnim() => animator.SetBool("IsRecoverable", true);

    public void ResetDeathAnim() => animator.ResetTrigger("Death");
}

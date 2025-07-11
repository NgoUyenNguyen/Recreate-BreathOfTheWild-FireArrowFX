using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ArcherVisual : MonoBehaviour
{
    private readonly int VELOCITY_HASH = Animator.StringToHash("Velocity");
    private readonly int SHOOTING_HASH = Animator.StringToHash("Shooting");
    private readonly int AIMING_HASH = Animator.StringToHash("Aiming");



    [SerializeField] private Archer _logic;
    [SerializeField] private GameObject _arrowVisual;
    private Animator _anim;





    public Animator Anim { get => _anim; set => _anim ??= value; }
    public Archer Logic { get => _logic;}
    private GameObject ArrowVisual { get => _arrowVisual;}

    private void Awake()
    {
        Anim = GetComponent<Animator>();
    }



    private void OnAnimatorMove()
    {
        Vector3 deltaPos = Anim.deltaPosition;
        Quaternion deltaRot = Anim.deltaRotation;

        Logic.transform.position += deltaPos;
        Logic.transform.rotation *= deltaRot;
    }







    public void MoveVisual(float velocity)
    {
        Anim.SetFloat(VELOCITY_HASH, velocity);
    }

    public void AimVisual(bool isAiming)
    {
        Anim.SetBool(AIMING_HASH, isAiming);
        ArrowVisual.SetActive(isAiming);
    }

    public void ShootVisual()
    {
        Anim.SetTrigger(SHOOTING_HASH);
    }
}

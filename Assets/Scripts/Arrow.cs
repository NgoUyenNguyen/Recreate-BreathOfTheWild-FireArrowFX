using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider), typeof(CinemachineImpulseSource))]
public class Arrow : StateManager<Arrow.EState>
{
    [SerializeField] private ArrowEffect _arrowEffect;
    private readonly float _maxForce = 100f;
    private readonly float _chargeTime = 1.5f;
    private float _currentForce;
    private bool _startShooting;
    private bool _isHit;
    private Rigidbody _arrowRB;
    private Collider _arrowCL;
    private CinemachineImpulseSource _impulseSource;
    private Archer _archer;

    public float MaxForce => _maxForce;
    public bool StartShooting { get => _startShooting; set => _startShooting = value; }
    public float CurrentForce { get => _currentForce; set => _currentForce = value; }
    public Rigidbody ArrowRB { get => _arrowRB; set => _arrowRB ??= value; }
    public bool IsHit { get => _isHit; private set => _isHit = value; }
    public float ChargeTime => _chargeTime;
    public Archer Archer { get => _archer; set => _archer = value; }
    public ArrowEffect ArrowEffect { get => _arrowEffect;}
    public Collider ArrowCL { get => _arrowCL; set => _arrowCL ??= value; }
    public CinemachineImpulseSource ImpulseSource { get => _impulseSource; set => _impulseSource ??= value; }

    public enum EState
    {
        Charging,
        Ready,
        Flying,
        Hit
    }









    protected override void OnAwake()
    {
        ArrowRB = GetComponent<Rigidbody>();
        ArrowRB.isKinematic = true;

        ArrowCL = GetComponent<Collider>();
        ArrowCL.enabled = false;

        ImpulseSource = GetComponent<CinemachineImpulseSource>();
    }

    private void FixedUpdate()
    {
        if (ArrowRB.linearVelocity != Vector3.zero)
        {
            transform.forward = ArrowRB.linearVelocity.normalized;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (CurrentState is Arrow_Flying flyingState)
        {
            IsHit = true;

            flyingState.HitTR = collision.transform;
        }
    }


    protected override EState InitializeEntryState()
    {
        return EState.Charging;
    }

    protected override void InitializeStates()
    {
        AddStates
            (
                new Arrow_Charging(EState.Charging, this),
                new Arrow_Ready(EState.Ready, this),
                new Arrow_Flying(EState.Flying, this),
                new Arrow_Hit(EState.Hit, this)
            );
    }

    public void StopArrow()
    {
        ArrowRB.linearVelocity = Vector3.zero;
        ArrowRB.angularVelocity = Vector3.zero;

        ArrowRB.isKinematic = true;
        IsHit = true;
    }
}










public class Arrow_Charging : BaseState<Arrow.EState>
{
    private readonly Arrow _context;
    





    public Arrow_Charging(Arrow.EState key, Arrow context) : base(key)
    {
        _context = context;
    }

    public Arrow Context => _context;

    public override void EnterState()
    {
        Context.CurrentForce = 0f;

        Context.ArrowEffect.PlayChargingEffects();
    }





    public override void ExitState()
    {
        
    }




    public override Arrow.EState GenerateNextState()
    {
        if (Context.StartShooting)
        {
            return Arrow.EState.Flying;
        }
        else if (Context.CurrentForce >= Context.MaxForce)
        {
            Context.CurrentForce = Context.MaxForce;
            return Arrow.EState.Ready;
        }

        return Arrow.EState.Charging;
    }



    public override void UpdateState()
    {
        Debug.Log("Charging arrow... Current Force: " + Context.CurrentForce);
        Context.CurrentForce += (Context.MaxForce / Context.ChargeTime) * Time.deltaTime;

        Debug.DrawLine(Context.transform.position, Context.Archer.ShootTargetPoint.position, Color.red);
    }
}











public class Arrow_Ready : BaseState<Arrow.EState>
{
    private readonly Arrow _context;
    
    
    
    public Arrow_Ready(Arrow.EState key, Arrow context) : base(key)
    {
        _context = context;
    }
    public Arrow Context => _context;
    
    public override void EnterState()
    {
        Context.ArrowEffect.PlayReadyEffects();
    }



    public override void ExitState()
    {
        Context.ArrowCL.enabled = false;

    }



    public override Arrow.EState GenerateNextState()
    {
        if (Context.StartShooting)
        {
            return Arrow.EState.Flying;
        }

        return Arrow.EState.Ready;
    }



    public override void UpdateState()
    {
    }
}













public class Arrow_Flying : BaseState<Arrow.EState>
{
    private readonly Arrow _context;

    private Transform _hitTR;




    public Arrow_Flying(Arrow.EState key, Arrow context) : base(key)
    {
        _context = context;
    }

    public Arrow Context => _context;
    public Transform HitTR { get => _hitTR; set => _hitTR = value; }

    public override void EnterState()
    {
        Context.ArrowCL.enabled = true;


        Context.ArrowRB.isKinematic = false;

        Context.transform.forward = Context.Archer.ShootTargetPoint.position - Context.transform.position;
        Context.ArrowRB.AddForce(Context.transform.forward * Context.CurrentForce, ForceMode.Impulse);
        
        Context.transform.SetParent(null);



        Context.ArrowEffect.StopChargingEffect();
        Context.ArrowEffect.StopReadyEffects();
        
        if (Context.CurrentForce >= Context.MaxForce)
        {
            Context.ArrowEffect.PlayFlyingEffects();

            Context.ImpulseSource.GenerateImpulse(Vector3.up * .1f);
        }
    }
    
    
    
    
    
    public override void ExitState()
    {
        Context.ArrowCL.enabled = false;

        Context.transform.SetParent(HitTR);

        Context.StopArrow();
    }
    
    
    
    
    
    
    public override Arrow.EState GenerateNextState()
    {
        if (Context.IsHit)
        {
            return Arrow.EState.Hit;
        }

        return Arrow.EState.Flying;
    }
    
    
    
    
    public override void UpdateState()
    {
        
    }
}











public class Arrow_Hit : BaseState<Arrow.EState>
{
    private readonly Arrow _context;
    
    
    
    
    public Arrow_Hit(Arrow.EState key, Arrow context) : base(key)
    {
        _context = context;
    }
    public Arrow Context => _context;
    
    public override void EnterState()
    {
        if (Context.CurrentForce >= Context.MaxForce)
        {
            Context.ArrowEffect.PlayHitEffects();
        }
       
    }



    public override void ExitState()
    {
    }
    
    
    
    public override Arrow.EState GenerateNextState()
    {
        return Arrow.EState.Hit;
    }
    
    
    
    public override void UpdateState()
    {
    }
}

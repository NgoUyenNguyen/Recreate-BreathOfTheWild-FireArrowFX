using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Arrow : StateManager<Arrow.EState>
{
    private readonly float _maxForce = 100f;
    private float _currentForce;
    private bool startShooting;
    private Rigidbody _arrowRB;

    public float MaxForce => _maxForce;
    public bool StartShooting { get => startShooting; set => startShooting = value; }
    public float CurrentForce { get => _currentForce; set => _currentForce = value; }
    public Rigidbody ArrowRB { get => _arrowRB; set => _arrowRB ??= value; }

    public enum EState
    {
        Charging,
        Ready,
        Flying,
        Hit
    }









    protected override void OnAwake()
    {
        _arrowRB = GetComponent<Rigidbody>();
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

    
}








public class Arrow_Charging : BaseState<Arrow.EState>
{
    private readonly Arrow _context;
    
    private readonly float _chargeTime = 5f;






    public Arrow_Charging(Arrow.EState key, Arrow context) : base(key)
    {
        _context = context;
    }

    public Arrow Context => _context;
    public float ChargeTime => _chargeTime;

    public override void EnterState()
    {
        Debug.Log("Arrow is charging.");
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

        return Arrow.EState.Ready;
    }



    public override void UpdateState()
    {
        Context.CurrentForce = Mathf.Lerp(Context.CurrentForce, Context.MaxForce, Time.deltaTime / ChargeTime);
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
        Debug.Log("Arrow is ready to shoot.");
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
    private Transform HitTR { get => _hitTR; set => _hitTR = value; }

    public override void EnterState()
    {
        Debug.Log("Arrow is flying.");
        Context.ArrowRB.AddForce(Context.ArrowRB.transform.forward * Context.CurrentForce, ForceMode.Impulse);
        Context.transform.SetParent(null);
    }
    
    
    
    
    
    public override void ExitState()
    {
        Context.transform.SetParent(HitTR);
    }
    
    
    
    
    
    
    public override Arrow.EState GenerateNextState()
    {
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
        Debug.Log("Arrow has hit the target.");
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

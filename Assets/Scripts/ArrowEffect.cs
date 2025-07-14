using UnityEngine;

public class ArrowEffect : MonoBehaviour
{
    [Header("Charging")]
    [SerializeField] private ParticleSystem _flashCharging;
    [SerializeField] private ParticleSystem _dustCharging;
    [SerializeField] private ParticleSystem _circleCharging;
    [SerializeField] private ParticleSystemForceField _forceFieldCharging;

    [Header("Ready")]
    [SerializeField] private ParticleSystem _popReady;
    [SerializeField] private ParticleSystem _flashReady;
    [SerializeField] private ParticleSystem _lightReady;

    [Header("Flying")]
    [SerializeField] private ParticleSystem _bustFlying;

    [Header("Hit")]
    [SerializeField] private ParticleSystem _explosionHit;
    [SerializeField] private ParticleSystem _dustHit;





    private ParticleSystem FlashCharging { get => _flashCharging; }
    private ParticleSystem DustCharging { get => _dustCharging; }
    private ParticleSystem CircleCharging { get => _circleCharging; }
    private ParticleSystem PopReady { get => _popReady; }
    private ParticleSystem FlashReady { get => _flashReady; }
    private ParticleSystem LightReady { get => _lightReady; }
    private ParticleSystem BustFlying { get => _bustFlying; }
    private ParticleSystem ExplosionHit { get => _explosionHit; }
    private ParticleSystem DustHit { get => _dustHit; }
    private ParticleSystemForceField ForceFieldCharging { get => _forceFieldCharging;}

    public void PlayChargingEffects()
    {
        ForceFieldCharging.enabled = true;

        FlashCharging.Play();
        DustCharging.Play();
        CircleCharging.Play();
    }

    public void StopChargingEffect()
    {
        ForceFieldCharging.enabled = false;

        FlashCharging.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        DustCharging.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        CircleCharging.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }



    public void PlayReadyEffects()
    {
        PopReady.Play();
        FlashReady.Play();
        LightReady.Play();
    }

    public void StopReadyEffects()
    {
        PopReady.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        FlashReady.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        LightReady.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }



    public void PlayFlyingEffects()
    {
        BustFlying.Play();
    }



    public void PlayHitEffects()
    {
        ExplosionHit.Play();
        DustHit.Play();
    }
}

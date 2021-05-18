using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
//
public class TwirlEffectManipulationExample : MonoBehaviour
{
	// Your Post-processing volume with effect;
	public Volume volume;
	public Slider amo;
	public Slider sp;
	public Slider rad ;

	public float Radius = 5;
	public float Amount;
	public float Speed;
	bool l = true;
	// Temp effect.
	private LimitlessTwirlEffefctURP effect;

	private void Start()
	{
		//Null check
		if (volume == null)
			return;

		// Get refference to effect
		volume.profile.TryGet(out effect);

		//Null check
		if (effect is null)
		{
			Debug.Log("Add effect to your Volume component to make Manipulation Example work");
			return;
		}

		//Activate effect
		effect.active = true;
	}
	public void chtype(bool val)
	{
		l = !l;
		effect.m_EffectType.value = l == true ? effectType.Twirl : effectType.Wave;
	}
	private void Update()
	{
		//Null check
		if (volume == null)
			return;
		if (effect is null)
			return;

		effect.amount.value = Amount;
		effect.speed.value = Speed;
		effect.effectRadius.value = Radius;

		//Debug.Log(amo.value + " 0.3 amo.value");
		//Debug.Log(sp.value + " 1.6 sp.value");
		//Debug.Log(rad.value + " 5  rad.value");
	}
}

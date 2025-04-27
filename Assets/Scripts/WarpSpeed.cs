using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class WarpSpeed : MonoBehaviour
{
    public VisualEffect warpSpeedVFX;
    public MeshRenderer cylinder;
    public SlideSpawner slideSpawner;
    public GameObject player;
    private bool warpActive;

    public float delay = 2.5f;
    public float rate;

    public Volume postProcessingVolume;
    private LensDistortion lensDistortion;
    public float transitionDurationLD = 2f;
    public float targetIntensity = -1f;
    private float originalIntensity;
    private void Start()
    {
        if (postProcessingVolume == null)
        {
            Debug.LogError("Post-Processing Volume is not assigned!");
            return;
        }

        postProcessingVolume.profile.TryGet(out lensDistortion);

        if (lensDistortion == null)
        {
            Debug.LogError("Lens Distortion effect not found in the Post-Processing Profile!");
            return;
        }

        originalIntensity = lensDistortion.intensity.value;
        warpSpeedVFX.Stop();
        warpSpeedVFX.SetFloat("WarpAmount", 0);

        cylinder.material.SetFloat("_Active_", 0);
    }
    public void WarpSpeedVFX(bool active)
    {
        warpActive = active;
        slideSpawner.GetBoolSpawnSlide(false);
        StartCoroutine(TransitionLensDistortion(targetIntensity));
        StartCoroutine(ActivateParticles());
        StartCoroutine(ActivateShader());
    }
    public void WarpSpeedVFXDeactivate()
    {
        //Deactivate warp speed VFX
        warpActive = false;
        StartCoroutine(TransitionLensDistortion(originalIntensity));
        StartCoroutine(ActivateParticles());
        StartCoroutine(ActivateShader());
        player.SetActive(true);
        slideSpawner.SpaceShipExit();
    }
    private IEnumerator TransitionLensDistortion(float targetValue)
    {
        float elapsedTime = 0f;
        float startValue = lensDistortion.intensity.value;

        while (elapsedTime < transitionDurationLD)
        {
            float t = elapsedTime / transitionDurationLD;
            lensDistortion.intensity.value = Mathf.Lerp(startValue, targetValue, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
    IEnumerator ActivateParticles()
    {
        if (warpActive)
        {
            warpSpeedVFX.Play();

            float amount = warpSpeedVFX.GetFloat("WarpAmount");
            while(amount < 1 & warpActive)
            {
                amount +=rate;
                warpSpeedVFX.SetFloat("WarpAmount",amount);
                yield return new WaitForSeconds(0.1f);
            }
        }
        else
        {
            //yield return new WaitForSeconds(2f);
            float amount = warpSpeedVFX.GetFloat("WarpAmount");
            while (amount > 0 & !warpActive)
            {
                amount = -rate;
                warpSpeedVFX.SetFloat("WarpAmount", amount);
                yield return new WaitForSeconds(0.1f);

                if(amount <= 0 + rate)
                {
                    amount = 0;
                    warpSpeedVFX.SetFloat("WarpAmount", amount);
                    warpSpeedVFX.Stop();
                }
            }
        }
    }
    IEnumerator ActivateShader()
    {
        if (warpActive)
        {
            yield return new WaitForSeconds(delay);
            float amount = cylinder.material.GetFloat("_Active_");
            while (amount < 1 & warpActive)
            {
                amount += rate;
                cylinder.material.SetFloat("_Active_", amount);
                yield return new WaitForSeconds(0.1f);
                if(amount > 1)
                {
                    WarpSpeedVFXDeactivate();
                    yield return new WaitForSeconds(0.5f);
                    slideSpawner.GetBoolSpawnSlide(true);
                }
            }
        }
        else
        {
            float amount = cylinder.material.GetFloat("_Active_");
            while (amount > 0 & !warpActive)
            {
                amount -= rate;
                cylinder.material.SetFloat("_Active_", amount);
                yield return new WaitForSeconds(0.1f);

                if (amount <= 0 + rate)
                {
                    amount = 0;
                    cylinder.material.SetFloat("_Active_", amount); 
                }
            }
        }
    }
}

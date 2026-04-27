using UnityEngine;

public class Shooting : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Transform BulletExitPoint;
    [SerializeField] private GameObject Bullet;
    [SerializeField] float delay = 1f;
    [SerializeField] private float recoilIntensity = 0.1f;
    [SerializeField] private float recoilDuration = 0.1f;

    private float lastShotTime = -Mathf.Infinity;
    private bool isShooting = false;

    void Update()
    {
        // Fire immediately on click and continue firing while holding,
        if ((Input.GetMouseButtonDown(0) || Input.GetMouseButton(0)) && Time.time - lastShotTime >= delay)
        {
            Shoot();
            lastShotTime = Time.time;
        }
        // Enemy shoots when it detects the player
        if (isShooting && Time.time - lastShotTime >= delay)
        {
            Shoot();
            lastShotTime = Time.time;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isShooting = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isShooting = false;
        }
    }
    void Shoot()
    {
        Instantiate(Bullet, BulletExitPoint.transform.position, BulletExitPoint.transform.rotation * Quaternion.Euler(0f, 0f, 90f));
        StartCoroutine(ApplyRecoil());
    }

    private System.Collections.IEnumerator ApplyRecoil()
    {
        Vector3 originalPos = transform.localPosition;
        float elapsedTime = 0f;

        while (elapsedTime < recoilDuration)
        {
            transform.localPosition = originalPos + Random.insideUnitSphere * recoilIntensity;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
    }
}

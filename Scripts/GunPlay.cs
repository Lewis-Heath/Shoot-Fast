using UnityEngine;

public class GunPlay : MonoBehaviour
{
    public float damage;
    public float fireRate;
    public float range;
    public float accuracy;
    public ParticleSystem muzzleFlash;
    public Light muzzleLight;
    public GameObject bulletImpact;
    public Camera fpsCam;
    private float timeToFire = 0f;
    private int impactCount;
    private float timeHeldDown;
    private float m_BulletSpreadMax = 0.2f;

    void Start()
    {
        muzzleFlash.Stop();
        muzzleLight.enabled = false;
    }

    void Update()
    {
        //Debug.Log(timeHeldDown);

        if(Input.GetButton("Fire1"))
        {
            timeHeldDown += Time.deltaTime;
            if (Time.time >= timeToFire)
            {
                timeToFire = Time.time + 1f / fireRate;
                Shoot();
            }
        }
        else
        {
            timeHeldDown = 0f;
        }

        if (Input.GetMouseButtonDown(0))
        {
            muzzleFlash.Play();
            muzzleLight.enabled = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            muzzleFlash.Stop();
            muzzleLight.enabled = false;
        }

    }

    void Shoot()
    {
        Vector3 bulletImpactPosition = fpsCam.transform.position;
        Vector2 bulletSpread = GetBulletSpread();
        bulletImpactPosition.x += bulletSpread.x;
        bulletImpactPosition.y += bulletSpread.y;

        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            GameObject[] activeBulletImpacts1 = GameObject.FindGameObjectsWithTag("BulletImpact");
            impactCount = activeBulletImpacts1.Length;

            if(impactCount > 10)
            {
                Destroy(activeBulletImpacts1[0].gameObject);
            }

            GameObject impact = Instantiate(bulletImpact, hit.point, Quaternion.LookRotation(hit.point));
            Destroy(impact.gameObject, 5f);

            Target target = hit.transform.GetComponent<Target>();
            if(target != null)
            {
                target.TakeDamage(damage);

                if (target.health <= 0)
                {
                    GameObject[] activeBulletImpacts2 = GameObject.FindGameObjectsWithTag("BulletImpact");
                    foreach (GameObject activeBulletImpact in activeBulletImpacts2)
                    {
                        Destroy(activeBulletImpact.gameObject);
                    }
                }
            }
        }
    }

    Vector2 GetBulletSpread()
    {
        Vector2 bulletSpread;
        float bulletSpreadMax = timeHeldDown * fireRate / accuracy;
        if (bulletSpreadMax > 0.2f)
        {
            bulletSpreadMax = 0.2f;
        }
        float bulletSpreadX = Random.Range(0, bulletSpreadMax);
        float bulletSpreadY = Random.Range(0, bulletSpreadMax);

        int randomX = Random.Range(0, 2);
        if (randomX == 1)
        {
            bulletSpread.x = bulletSpreadX;
        }
        else
        {
            bulletSpread.x = -bulletSpreadX;
        }

        int randomY = Random.Range(0, 2);
        if(randomY == 1)
        {
            bulletSpread.y = bulletSpreadY;
        }
        else
        {
            bulletSpread.y = -bulletSpreadY;
        }

        //Debug.Log("X offset: "+ bulletSpreadX);
        //Debug.Log("Y offset: " + bulletSpreadY);
        //Debug.Log("Bullet spread max: " + bulletSpreadMax);

        return bulletSpread;
    }
}

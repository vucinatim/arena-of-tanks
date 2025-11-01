using UnityEngine;
using System.Collections;

public class VolumetricComponent : ExploderComponent
{
    public float duration = 2;
    public float centerEmission = 20000;
    public float centerEmissionDuration = 0.2f;
    public float radius = 0;
    public int startEmission = 3000;
    public int emission = 3000;
    public int maxParticles = 100000;
    public Gradient colorOverLifetime;
    public AnimationCurve alphaOverLifetime = AnimationCurve.EaseInOut(0, 1, 1, 0);
    public float particleSizeMultiplier = 4;
    public int teleportationIterations = 4;
    public float teleportationThreshold = 1.5f;

    protected Exploder exploder;

    protected ParticleSystem.Particle[] particles;
    protected Vector3[] directions;
    protected int[] hitCount;
    protected float speed;
    protected int curCount = 0;

    public override void onExplosionStarted(Exploder exploder)
    {
        particles = new ParticleSystem.Particle[maxParticles];
        directions = new Vector3[maxParticles];
        hitCount = new int[maxParticles];

        if (GetComponent<ParticleSystem>() == null)
        {
            gameObject.AddComponent<ParticleSystem>();
        }
        this.exploder = exploder;
        if (radius < 0.0001f)
        {
            radius = exploder.radius;
        }
        speed = radius / duration;

        InitParticleSystem();

        StartCoroutine("emulate");
    }

    private void InitParticleSystem()
    {
        ParticleSystem.MainModule main = GetComponent<ParticleSystem>().main;
        ParticleSystem.EmissionModule emissionModule = GetComponent<ParticleSystem>().emission;

        main.maxParticles = maxParticles;
        emissionModule.rateOverTime = 0;
        main.startSpeed = 0;
        main.startSize = 1.0f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.startLifetime = duration;

        GetComponent<ParticleSystem>().Emit(startEmission);
        curCount = GetComponent<ParticleSystem>().GetParticles(particles);

        for (int i = 0; i < curCount; i++)
        {
            directions[i] = Random.onUnitSphere;
            particles[i].position = transform.position;
            particles[i].startColor = colorOverLifetime.Evaluate(0);
        }

        GetComponent<ParticleSystem>().SetParticles(particles, curCount);
    }

    protected void emitNewParticles()
    {
        if ((Time.time - exploder.explosionTime) / duration < centerEmissionDuration)
        {
            GetComponent<ParticleSystem>()
                .Emit(Mathf.Min((int)(centerEmission * Time.deltaTime), maxParticles - curCount));
        }
        else
        {
            GetComponent<ParticleSystem>()
                .Emit(Mathf.Min((int)(emission * Time.deltaTime), maxParticles - curCount));
        }
        int nextCount = GetComponent<ParticleSystem>().GetParticles(particles);

        if ((Time.time - exploder.explosionTime) / duration < centerEmissionDuration)
        {
            for (int i = curCount; i < nextCount; i++)
            {
                directions[i] = Random.onUnitSphere;
                particles[i].position = transform.position;
                MoveParticle(
                    i,
                    Mathf.Max(
                        Time.time - exploder.explosionTime - Time.deltaTime,
                        Time.deltaTime * 0.1f
                    ) * speed
                );
            }
        }
        else
        {
            float emitAngle = Random.Range(Mathf.PI / 6, Mathf.PI / 3);
            for (int i = curCount; i < nextCount; i++)
            {
                int copyId = Random.Range(0, curCount);
                directions[i] = GetAlignedDirection(copyId, emitAngle);
                particles[i].position = particles[copyId].position;
                hitCount[i] = hitCount[copyId];
            }
        }

        curCount = nextCount;
    }

    private void TeleportBadParticles()
    {
        float copyAngle = Random.Range(0, Mathf.PI / 6);
        for (int i = 0; i < curCount; i++)
        {
            int copyId = Random.Range(0, curCount);
            if (hitCount[copyId] * teleportationThreshold < hitCount[i])
            {
                directions[i] = GetAlignedDirection(copyId, copyAngle);
                particles[i].position = particles[copyId].position;
                hitCount[i] = hitCount[copyId] + 1;
            }
        }
    }

    private Vector3 GetAlignedDirection(int id, float angle)
    {
        float beta;
        beta = Random.Range(0, 2 * Mathf.PI);
        Vector3 randomDir = new Vector3(
            Mathf.Sin(angle) * Mathf.Cos(beta),
            Mathf.Sin(angle) * Mathf.Sin(beta),
            Mathf.Cos(angle)
        );
        Quaternion rotation = Quaternion.LookRotation(randomDir);
        return rotation * directions[id];
    }

    protected void MoveParticle(int id, float distance)
    {
        Ray testRay = new(particles[id].position, directions[id]);

        RaycastHit hit;
        if (Physics.Raycast(testRay, out hit, distance))
        {
            if (!hit.rigidbody)
            {
                Vector3 reflectVec = Random.onUnitSphere;
                if (Vector3.Dot(reflectVec, hit.normal) < 0)
                {
                    reflectVec *= -1;
                }
                directions[id] = reflectVec;
            }
            particles[id].position = testRay.origin + testRay.direction * hit.distance * 0.95f;
            hitCount[id]++;
        }
        else
        {
            particles[id].position = testRay.origin + testRay.direction * distance;
        }
    }
}

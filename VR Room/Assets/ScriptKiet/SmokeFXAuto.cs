using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(ParticleSystem))]
public class SmokeFX_ColumnTight : MonoBehaviour
{
    [Tooltip("Tùy chọn: kéo material khói (Alpha blend, KHÔNG Additive) vào đây.")]
    public Material overrideMaterial;

    [ContextMenu("Apply Column Smoke Preset")]
    public void Apply()
    {
        var ps = GetComponent<ParticleSystem>();

        // MAIN — gọn, không quá sáng
        var main = ps.main;
        main.loop = true;
        main.simulationSpace = ParticleSystemSimulationSpace.Local;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.9f, 1.4f);
        main.startSpeed    = new ParticleSystem.MinMaxCurve(0.05f, 0.12f);
        main.startSize     = new ParticleSystem.MinMaxCurve(0.06f, 0.10f);
        main.startColor    = Color.white;
        main.maxParticles  = 40;

        // EMISSION — vừa đủ để không lố
        var emi = ps.emission;
        emi.enabled = true;
        emi.rateOverTime = 5f;

        // SHAPE — cone hẹp, bám hướng lên
        var shape = ps.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle  = 6f;          // hẹp lại
        shape.radius = 0.008f;      // nhỏ
        shape.alignToDirection = true;

        // VELOCITY OVER LIFETIME — kéo thẳng lên
        var vol = ps.velocityOverLifetime;
        vol.enabled = true;
        vol.space = ParticleSystemSimulationSpace.Local;
        vol.x = new ParticleSystem.MinMaxCurve(0f);
        vol.y = new ParticleSystem.MinMaxCurve(0.25f); // lên trên
        vol.z = new ParticleSystem.MinMaxCurve(0f);

        // FORCE OVER LIFETIME — lực nâng nhẹ, giữ cột
        var fol = ps.forceOverLifetime;
        fol.enabled = true;
        fol.space = ParticleSystemSimulationSpace.World;
        fol.x = new ParticleSystem.MinMaxCurve(0f);
        fol.y = new ParticleSystem.MinMaxCurve(0.10f); // nâng dọc trục Y
        fol.z = new ParticleSystem.MinMaxCurve(0f);

        // LIMIT VELOCITY — kìm tốc độ ngang để không tỏa loạn
        var lim = ps.limitVelocityOverLifetime;
        lim.enabled = true;
        lim.separateAxes = false;
        lim.limit = new ParticleSystem.MinMaxCurve(0.25f);
        lim.dampen = 0.8f; // giảm rung ngang

        // COLOR OVER LIFETIME — xám dịu, không trắng gắt
        var col = ps.colorOverLifetime;
        col.enabled = true;
        var g = new Gradient();
        g.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(Hex("#777777"), 0.00f),
                new GradientColorKey(Hex("#8F8F8F"), 0.60f),
                new GradientColorKey(Hex("#9F9F9F"), 1.00f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(0.22f, 0.00f),
                new GradientAlphaKey(0.12f, 0.60f),
                new GradientAlphaKey(0.00f, 1.00f)
            }
        );
        col.color = new ParticleSystem.MinMaxGradient(g);

        // SIZE OVER LIFETIME — giữ cột thon
        var sol = ps.sizeOverLifetime;
        sol.enabled = true;
        sol.separateAxes = false;
        sol.size = new ParticleSystem.MinMaxCurve(
            1f,
            new AnimationCurve(
                new Keyframe(0f, 0.85f),
                new Keyframe(1f, 1.25f)  // nở nhẹ
            )
        );

        // NOISE — rất thấp, tránh bay loạn
        var noise = ps.noise;
        noise.enabled = true;
        noise.strength = 0.03f;     // thấp
        noise.frequency = 0.5f;
        noise.scrollSpeed = 0.10f;
        noise.damping = true;
        noise.quality = ParticleSystemNoiseQuality.Low;

        // RENDERER — dùng Alpha blend để khỏi “trắng gắt”
        var r = GetComponent<ParticleSystemRenderer>();
        if (r)
        {
            r.renderMode = ParticleSystemRenderMode.Billboard;
            r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            r.receiveShadows = false;
            r.sortingFudge = 1f;

            if (overrideMaterial) r.sharedMaterial = overrideMaterial;
        }

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(ps);
        if (r) UnityEditor.EditorUtility.SetDirty(r);
#endif
    }

    static Color Hex(string hex)
    {
        Color c; ColorUtility.TryParseHtmlString(hex, out c); return c;
    }

    void Reset()      => Apply();
    void Awake()      { if (Application.isPlaying) Apply(); }
    void OnValidate() { if (!Application.isPlaying) Apply(); }
}
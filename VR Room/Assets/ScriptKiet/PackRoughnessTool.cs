#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;

public class PackRoughnessTool : EditorWindow
{
    enum Pipeline { BuiltIn, URP }
    Pipeline pipeline = Pipeline.BuiltIn;

    Texture2D roughness;  // sRGB Off
    Texture2D metallicTex; // optional, sRGB Off
    float metallicScalar = 0f; // dùng nếu không có metallicTex
    Texture2D aoTex; // optional (URP), sRGB Off

    string outputName = "PackedMask";

    [MenuItem("Tools/Textures/Pack Roughness → Smoothness (Alpha)")]
    static void Open() => GetWindow<PackRoughnessTool>("Pack Roughness");

    void OnGUI()
    {
        GUILayout.Label("Pipeline", EditorStyles.boldLabel);
        pipeline = (Pipeline)EditorGUILayout.EnumPopup("Target", pipeline);

        GUILayout.Space(6);
        GUILayout.Label("Inputs", EditorStyles.boldLabel);
        roughness = (Texture2D)EditorGUILayout.ObjectField("Roughness (Linear)", roughness, typeof(Texture2D), false);
        metallicTex = (Texture2D)EditorGUILayout.ObjectField("Metallic (Linear, optional)", metallicTex, typeof(Texture2D), false);
        if (metallicTex == null)
            metallicScalar = EditorGUILayout.Slider("Metallic (0–1)", metallicScalar, 0f, 1f);

        if (pipeline == Pipeline.URP)
            aoTex = (Texture2D)EditorGUILayout.ObjectField("AO (Linear, optional)", aoTex, typeof(Texture2D), false);

        outputName = EditorGUILayout.TextField("Output Name", outputName);

        GUILayout.Space(8);
        using (new EditorGUI.DisabledScope(roughness == null))
        {
            if (GUILayout.Button("Generate"))
                Generate();
        }

        EditorGUILayout.HelpBox(
            "Nhớ để sRGB Off cho roughness/metallic/AO trước khi chạy.\n" +
            "Built-in: R=Metallic, A=Smoothness. URP: R=Metallic, G=AO, B=1, A=Smoothness.",
            MessageType.Info);
    }

    void Generate()
    {
        var rRef = GetReadable(roughness);
        int W = rRef.width, H = rRef.height;

        var mRef = metallicTex ? ResizeTo(GetReadable(metallicTex), W, H) : null;
        var aoRef = aoTex ? ResizeTo(GetReadable(aoTex), W, H) : null;

        var outTex = new Texture2D(W, H, TextureFormat.RGBA32, true, /*linear*/ true);
        var rp = rRef.GetPixels();
        var mp = mRef ? mRef.GetPixels() : null;
        var ap = aoRef ? aoRef.GetPixels() : null;

        for (int i = 0; i < rp.Length; i++)
        {
            float rough = rp[i].r;                 // assume grayscale in R
            float smooth = 1f - Mathf.Clamp01(rough);

            float metallic = mp != null ? mp[i].r : metallicScalar;
            metallic = Mathf.Clamp01(metallic);

            if (pipeline == Pipeline.BuiltIn)
            {
                outTex.SetPixel(i % W, i / W, new Color(metallic, 0f, 0f, smooth));
            }
            else // URP
            {
                float ao = ap != null ? ap[i].r : 1f;
                ao = Mathf.Clamp01(ao);
                outTex.SetPixel(i % W, i / W, new Color(metallic, ao, 1f, smooth));
            }
        }

        outTex.Apply(true, false);

        string path = EditorUtility.SaveFilePanelInProject("Save packed map", outputName, "png", "Choose save location in Assets/");
        if (string.IsNullOrEmpty(path)) return;

        File.WriteAllBytes(path, outTex.EncodeToPNG());
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);

        // Import settings for packed map
        var ti = (TextureImporter)AssetImporter.GetAtPath(path);
        ti.sRGBTexture = false;                 // Mask phải ở Linear
        ti.wrapMode = TextureWrapMode.Repeat;
        ti.filterMode = FilterMode.Trilinear;
        ti.mipmapEnabled = true;
        ti.anisoLevel = 4;
        AssetDatabase.WriteImportSettingsIfDirty(path);
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("Done", "Packed map generated:\n" + path, "OK");
    }

    static Texture2D GetReadable(Texture2D src)
    {
        var rt = RenderTexture.GetTemporary(src.width, src.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
        Graphics.Blit(src, rt);
        var prev = RenderTexture.active;
        RenderTexture.active = rt;
        var tex = new Texture2D(src.width, src.height, TextureFormat.RGBA32, false, true);
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        tex.Apply();
        RenderTexture.active = prev;
        RenderTexture.ReleaseTemporary(rt);
        return tex;
    }

    static Texture2D ResizeTo(Texture2D src, int w, int h)
    {
        if (src.width == w && src.height == h) return src;
        var rt = RenderTexture.GetTemporary(w, h, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
        Graphics.Blit(src, rt);
        var prev = RenderTexture.active;
        RenderTexture.active = rt;
        var tex = new Texture2D(w, h, TextureFormat.RGBA32, false, true);
        tex.ReadPixels(new Rect(0, 0, w, h), 0, 0);
        tex.Apply();
        RenderTexture.active = prev;
        RenderTexture.ReleaseTemporary(rt);
        return tex;
    }
}
#endif

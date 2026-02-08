using System.IO;
using System.Text;
using UnityEditor.AssetImporters;
using UnityEngine;

[ScriptedImporter(1, "tsv")]
public class TsvImporter : ScriptedImporter
{
    public override void OnImportAsset(AssetImportContext ctx)
    {
        // ï∂éöâªÇØëŒçÙÇ≈UTF8éwíË
        var text = File.ReadAllText(ctx.assetPath, Encoding.UTF8);

        var ta = new TextAsset(text);
        ctx.AddObjectToAsset("text", ta);
        ctx.SetMainObject(ta);
    }
}
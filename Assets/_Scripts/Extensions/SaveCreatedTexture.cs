using UnityEditor;
using UnityEngine;

public static class SaveCreatedTexture
{
    private const string TextureFolderPath = "Assets/Textures/Generated";
    
    public static void SaveTextureToFile(this Texture2D texture, ObjectGenerator @object)
    {
        
        if (!System.IO.Directory.Exists(TextureFolderPath))
            System.IO.Directory.CreateDirectory(TextureFolderPath);

        // save texture to file in the folder we just created
        var fullSavePath = GetSavePath(@object.name);
        var textureBytes = texture.EncodeToPNG();
        System.IO.File.WriteAllBytes(fullSavePath, textureBytes);

        // refresh asset database to make sure the file we just saved shows up in the project
        AssetDatabase.Refresh();
    }
    
     public static void AddTexture(this Material material, ObjectGenerator @object)
     {
         // add the saved texture to the material
         var newTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(GetSavePath(@object.name));
         material.SetTexture("_Planet_Texture", newTexture);
     }

     private static string GetSavePath(string assetName)
     {
         return $"{TextureFolderPath}/{assetName}" + ".png";
     }
}
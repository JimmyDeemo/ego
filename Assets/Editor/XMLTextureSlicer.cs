using System;
using System.Collections.Generic;
using System.Xml;
using UnityEditor;
using UnityEngine;

public class XMLTextureSlicer : EditorWindow {

    public TextureImporter Importer;

    private TextAsset XmlFile;

    public SpriteAlignment Alignment = SpriteAlignment.Center;
    public Vector2 CustomOffset = new Vector2(0.5f, 0.5f);

    public void OnGUI()
    {
        XmlFile = EditorGUILayout.ObjectField("Slice Data (XML)", XmlFile, typeof(TextAsset), false) as TextAsset;

        Alignment = (SpriteAlignment) EditorGUILayout.EnumPopup("Pivot", Alignment);

        bool enabled = GUI.enabled; //Always cache the main GUI state...

        if (Alignment != SpriteAlignment.Custom)
        {
            GUI.enabled = false;
        }

        EditorGUILayout.Vector2Field("Custom Offset", CustomOffset);


        GUI.enabled = enabled; //...return it back.

        if (XmlFile == null)
        {
            GUI.enabled = false;
        }

        if (GUILayout.Button("Slice"))
        {
            Slice(); 
        }

        GUI.enabled = enabled;
    }

    private void Slice()
    {
        // This is where we actuall do the slicing.
        XmlDocument document = new XmlDocument();
        document.LoadXml(XmlFile.text);

        XmlElement root = document.DocumentElement;
        if (root.Name != "TextureAtlas")
        {
            Debug.LogError("Unable to find root node of 'TextureAtlas'.");
            return;
        }

        bool failed = false;

        Texture2D texture = AssetDatabase.LoadMainAssetAtPath(Importer.assetPath) as Texture2D;
        int textureHeight = texture.height;

        List<SpriteMetaData> spriteMetaData = new List<SpriteMetaData>(root.ChildNodes.Count);
        foreach (XmlNode node in root.ChildNodes)
        {
            if (node.Name != "SubTexture")
            {
                Debug.LogWarningFormat("Skipping '{0}' as it is not a 'SubTexture'.", node.Name);
                continue;
            }

            try
            {
                int width = Convert.ToInt32(node.Attributes["width"].Value);
                int height = Convert.ToInt32(node.Attributes["height"].Value);
                int x = Convert.ToInt32(node.Attributes["x"].Value);
                // Unity y == 0 is the opposite of the XML.
                int y = textureHeight - (height + Convert.ToInt32(node.Attributes["y"].Value));

                SpriteMetaData metaData = new SpriteMetaData
                {
                    alignment = (int)Alignment,
                    border = new Vector4(),
                    name = node.Attributes["name"].Value,
                    pivot = GetPivotValue(Alignment, CustomOffset),
                    rect = new Rect(x, y, width, height),
                };

                spriteMetaData.Add(metaData);
            }
            catch (System.Exception ex)
            {
                failed = true;
                Debug.LogException(ex);
            }
        }
                

        if (!failed)
        {
            Undo.RecordObject(Importer, "Slice Sprite with XML");
            Importer.spriteImportMode = SpriteImportMode.Multiple;
            Importer.spritesheet = spriteMetaData.ToArray();

            try
            {
                AssetDatabase.StartAssetEditing();
                AssetDatabase.ImportAsset(Importer.assetPath);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
                Close();
            }
        }
    }

    public static Vector2 GetPivotValue(SpriteAlignment alignment, Vector2 customOffset)
    {
        switch (alignment)
        {
            case SpriteAlignment.Center:
                return new Vector2(0.5f, 0.5f);
            case SpriteAlignment.TopLeft:
                return new Vector2(0.0f, 1.0f);
            case SpriteAlignment.TopCenter:
                return new Vector2(0.5f, 1.0f);
            case SpriteAlignment.TopRight:
                return new Vector2(1.0f, 1.0f);
            case SpriteAlignment.LeftCenter:
                return new Vector2(0.0f, 0.5f);
            case SpriteAlignment.RightCenter:
                return new Vector2(1.0f, 0.5f);
            case SpriteAlignment.BottomLeft:
                return new Vector2(0.0f, 0.0f);
            case SpriteAlignment.BottomCenter:
                return new Vector2(0.5f, 0.0f);
            case SpriteAlignment.BottomRight:
                return new Vector2(1.0f, 0.0f);
            case SpriteAlignment.Custom:
                return customOffset;
            default:
                return new Vector2(0.5f, 0.5f); //Default to center.
        }
    }

    #region Menu Item
    [MenuItem("CONTEXT/TextureImporter/Slice Using XML", true)]
    public static bool ValidateSliceUsingXML(MenuCommand command)
    {
        TextureImporter importer = command.context as TextureImporter;

        //Only want to show this menu option if the texture is a 'sprite' or 'advanced'.
        return importer && (importer.textureType == TextureImporterType.Sprite || importer.textureType == TextureImporterType.Default);
    }

    [MenuItem("CONTEXT/TextureImporter/Slice Using XML")]
    public static void SliceUsingXML(MenuCommand command)
    {
        XMLTextureSlicer window = GetWindow<XMLTextureSlicer>(true, "XML Slicer", true);
        window.Importer = command.context as TextureImporter;
        window.Show();
    }
    #endregion
}
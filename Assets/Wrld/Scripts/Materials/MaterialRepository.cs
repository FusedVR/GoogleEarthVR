using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Wrld.Materials
{
    public class MaterialRepository
    {
        public delegate void ApplyTextureToMaterialCallback(IntPtr materialName, uint textureID);

        public MaterialRepository(string materialsDirectory, Material defaultLandmarkMaterial, TextureLoadHandler textureLoadHandler)
        {
            m_materialDirectory = materialsDirectory;
            m_materials = new Dictionary<string, MaterialRecord>();
            m_defaultMaterial = defaultLandmarkMaterial == null ? LoadPlaceHolderMaterial("placeholder") : defaultLandmarkMaterial;
            m_defaultRasterTerrainMaterial = LoadOrCreateMaterial("placeholder", "placeholder_rasterTerrain");
            m_textureLoadHandler = textureLoadHandler;
        }

        public void Update()
        {
            var toRemove = new HashSet<Material>();

            foreach (var material in m_materialsRequiringTexture)
            {
                if (TryAssignTextureForMaterial(material))
                {
                    toRemove.Add(material);
                }
            }

            m_materialsRequiringTexture.RemoveWhere(_m => toRemove.Contains(_m));
        }

        private bool TryAssignTextureForMaterial(Material material)
        {
            var texture = GetTextureIDForMaterial(NativePluginRunner.API, material.name);

            if (texture != 0)
            {
                m_textureLoadHandler.Update();
                ApplyTextureToMaterial(material, m_textureLoadHandler.GetTexture(texture));

                return true;
            }

            return false;
        }

        private void ApplyTextureToMaterial(Material material, Texture texture)
        {
            material.SetTexture(Shader.PropertyToID("_MainTex"), texture);
        }

        Color RandomGrayColor()
        {
            var value = UnityEngine.Random.value;
            return new Color(value, value, value);
        }

        public Material LoadPlaceHolderMaterial(string placeholderName)
        {
            Material material = null;

            if (!string.IsNullOrEmpty(m_materialDirectory))
            {
                material = (Material)UnityEngine.Resources.Load(Path.Combine(m_materialDirectory, placeholderName), typeof(Material));

                if (material == null)
                {
                    throw new ArgumentException("Cannot find placeholder material or material directory is inaccessible");
                } 
            }
            else
            {
                material = new Material(Shader.Find("Standard"));
                material.SetColor("_Color", RandomGrayColor());
            }

            return material;
        }

        [DllImport(NativePluginRunner.DLL)]
        private static extern uint GetTextureIDForMaterial(
            IntPtr api, 
            [MarshalAs(UnmanagedType.LPStr)] string materialName);

        private static bool RequiresStreamedTexture(string materialName)
        {
            return materialName.StartsWith("Raster") || materialName.StartsWith("landmark_");
        }

        private static string GetDisambiguatedMaterialName(string objectID, string materialName)
        {
            return materialName.StartsWith("landmark_") ? materialName + "_" + objectID : materialName;
        }

        public Material LoadOrCreateMaterial(string objectID, string materialName)
        {
            MaterialRecord record;
            string disambiguatedMaterialName = GetDisambiguatedMaterialName(objectID, materialName);
            
            if (m_materials.TryGetValue(disambiguatedMaterialName, out record))
            {
                record.ReferenceCount++;

                return record.Material;
            }

            Material material = null;
            bool requiresTexture = RequiresStreamedTexture(materialName);

            if (!string.IsNullOrEmpty(m_materialDirectory))
            {
                material = (Material)UnityEngine.Resources.Load(Path.Combine(m_materialDirectory, materialName), typeof(Material));

                if (material == null)
                {
                    if (materialName == "placeholder_terrain")
                    {
                        material = m_defaultRasterTerrainMaterial;
                    }
                    else
                    {
                        bool isRasterMaterial = materialName.StartsWith("Raster");
                        var sourceMaterial = isRasterMaterial ? m_defaultRasterTerrainMaterial : m_defaultMaterial;
                        material = new Material(sourceMaterial);
                        material.CopyPropertiesFromMaterial(sourceMaterial);
                    }
                }
            }
            else
            {
                material = new Material(Shader.Find("Standard"));

                if (!materialName.StartsWith("landmark"))
                {
                    material.SetColor("_Color", RandomGrayColor());
                }
            }

            material.name = disambiguatedMaterialName;
            record = new MaterialRecord { Material = material, ReferenceCount = 1 };
            m_materials[disambiguatedMaterialName] = record;

            if (requiresTexture)
            {
                if (!TryAssignTextureForMaterial(material))
                {
                    m_materialsRequiringTexture.Add(material);
                }
            }

            return material;
        }

        public void ReleaseMaterial(string materialName)
        {
            if (!RequiresStreamedTexture(materialName))
            {
                return;
            }

            MaterialRecord record;

            if (m_materials.TryGetValue(materialName, out record))
            {
                if (--record.ReferenceCount == 0)
                {
                    if (m_materialsRequiringTexture.Contains(record.Material))
                    {
                        m_materialsRequiringTexture.Remove(record.Material);
                    }

                    m_materials.Remove(materialName);
                    GameObject.DestroyImmediate(record.Material);
                }
            }
            else
            {
                Debug.LogWarning(string.Format("material {0} was not present!", materialName));
            }
        }
        struct ApplyTextureToMaterialRequest
        {
            public string MaterialName { get; set; }
            public uint TextureID { get; set; }
        }

        class MaterialRecord
        {
            public Material Material { get; set; }
            public uint ReferenceCount { get; set; }
        }

        private HashSet<Material> m_materialsRequiringTexture = new HashSet<Material>();

        private Dictionary<string, MaterialRecord> m_materials;
        private Material m_defaultMaterial;
        private Material m_defaultRasterTerrainMaterial;
        private TextureLoadHandler m_textureLoadHandler;
        private string m_materialDirectory = null;
    };

}

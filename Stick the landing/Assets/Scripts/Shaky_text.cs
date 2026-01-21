using UnityEngine;
using TMPro;

public class Shaky_text : MonoBehaviour
{

    public TMP_Text text_component;

    private Vector3 initial_position;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initial_position = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        text_component.ForceMeshUpdate();
        var textinfo = text_component.textInfo;
        
        for(int i = 0; i < textinfo.characterCount; i++)
        {
            var charinfo = textinfo.characterInfo[i];

            if(!charinfo.isVisible)
            {
                continue;
            }

            var verts = textinfo.meshInfo[charinfo.materialReferenceIndex].vertices;

            for(int j = 0; j < 4; j++)
            {
                var orig = verts[charinfo.vertexIndex + j];
                verts[charinfo.vertexIndex + j] = orig + new Vector3(0, Mathf.Sin(Time.time * 8f + orig.x * 0.01f) * 1f, 0);
            }
        }
        for (int i = 0; i < textinfo.meshInfo.Length; i++) 
        {
            var meshinfo = textinfo.meshInfo[i];
            meshinfo.mesh.vertices = meshinfo.vertices;
            text_component.UpdateGeometry(meshinfo.mesh, i);
        }
    }
}

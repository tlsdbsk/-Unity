using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using PA2.Portfolio;

public static class PortfolioBuilder
{
    const string Root = "Assets/Portfolio";
    const string ScenePath = Root + "/Scenes/RelayRun.unity";
    static Material floor, edge, cyan, coral, white, dark, gold;

    [MenuItem("PA2/Build Portfolio Scene")]
    public static void CreateScene()
    {
        Directory.CreateDirectory(Root + "/Scenes");
        Directory.CreateDirectory(Root + "/Materials");
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
        floor = Mat("Deck", new Color(.065f,.11f,.16f));
        edge = Mat("Steel", new Color(.14f,.23f,.29f));
        dark = Mat("Dark", new Color(.022f,.038f,.058f));
        cyan = Mat("SignalCyan",new Color(.15f,.85f,.71f),1.1f);
        coral = Mat("HazardCoral",new Color(.95f,.18f,.13f),.6f);
        white = Mat("White",new Color(.7f,.83f,.88f));
        gold = Mat("Amber",new Color(1,.65f,.17f),.5f);
        RenderSettings.skybox = null;
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(.42f,.5f,.62f);
        RenderSettings.fog = true; RenderSettings.fogColor = new Color(.035f,.06f,.095f);
        RenderSettings.fogMode = FogMode.Linear; RenderSettings.fogStartDistance=40; RenderSettings.fogEndDistance=100;
        var light = new GameObject("Key Light").AddComponent<Light>(); light.type=LightType.Directional;
        light.intensity=1.25f; light.color=new Color(.72f,.85f,1); light.transform.rotation=Quaternion.Euler(48,-35,0);
        light.shadows=LightShadows.Soft;

        var session = new GameObject("Run Session").AddComponent<RunSession>();
        var hudObj = new GameObject("Interface",typeof(RectTransform)); session.hud=hudObj.AddComponent<RunHUD>();
        var cameraObject=new GameObject("Main Camera"); cameraObject.tag="MainCamera";
        var camera=cameraObject.AddComponent<Camera>(); camera.clearFlags=CameraClearFlags.SolidColor; camera.backgroundColor=RenderSettings.fogColor;
        camera.orthographic=true; camera.orthographicSize=12; camera.nearClipPlane=.1f; camera.farClipPlane=180; camera.allowHDR=true;
        cameraObject.AddComponent<AudioListener>(); session.view=cameraObject.AddComponent<RunCamera>();
        cameraObject.transform.position=new Vector3(-3,20,1); cameraObject.transform.rotation=Quaternion.Euler(50,0,0);

        // The original supplied player model is reused, with a separate controller for this course.
        var actor=new GameObject("Runner"); actor.transform.position=new Vector3(0,.1f,3); actor.tag="Player";
        var controller=actor.AddComponent<CharacterController>(); controller.center=new Vector3(0,.9f,0); controller.height=1.8f; controller.radius=.38f; controller.stepOffset=.3f;
        var runner=actor.AddComponent<Runner>(); session.player=runner; session.view.target=actor.transform;
        var playerPrefab=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Player/QuarterView 3D Action BE5/Prefabs/Player.prefab");
        if(!playerPrefab) throw new Exception("Original player model is missing");
        var model=(GameObject)PrefabUtility.InstantiatePrefab(playerPrefab); model.name="Original Player Visual"; model.transform.SetParent(actor.transform,false); model.transform.localScale=Vector3.one*.75f;
        runner.model=model.transform;
        var anim=model.GetComponentInChildren<Animator>();
        if(anim) { anim.runtimeAnimatorController=AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>("Assets/Player/Player.controller"); anim.applyRootMotion=false; runner.animator=anim; }
        var trailObject=new GameObject("Dash Trail"); trailObject.transform.SetParent(actor.transform,false); trailObject.transform.localPosition=new Vector3(0,.7f,0);
        var trail=trailObject.AddComponent<TrailRenderer>(); trail.sharedMaterial=cyan; trail.time=.22f; trail.startWidth=.65f; trail.endWidth=0; trail.emitting=false; trail.minVertexDistance=.05f; runner.trail=trail;
        var marker=Shape("Runner Beacon",PrimitiveType.Cylinder,new Vector3(0,.045f,3),new Vector3(1.2f,.018f,1.2f),cyan,false);
        marker.transform.SetParent(actor.transform,true);

        var geometry=new GameObject("Course Geometry").transform;
        for(int sector=0;sector<3;sector++)
        {
            float center=18+sector*34;
            Shape("Sector "+(sector+1)+" Deck",PrimitiveType.Cube,new Vector3(0,-.6f,center),new Vector3(16,1.2f,34),floor,true,geometry);
            for(int side=-1;side<=1;side+=2)
            {
                Shape("Edge Rail",PrimitiveType.Cube,new Vector3(side*8,.35f,center),new Vector3(.28f,.7f,34),edge,true,geometry);
                Shape("Luminous Trim",PrimitiveType.Cube,new Vector3(side*7.82f,.74f,center),new Vector3(.08f,.045f,34),cyan,false,geometry);
                for(int j=0;j<4;j++)
                {
                    Shape("Support",PrimitiveType.Cube,new Vector3(side*8.35f,-2,center-13+j*9),new Vector3(.7f,4,.9f),edge,false,geometry);
                    Shape("Service Tower",PrimitiveType.Cube,new Vector3(side*12,-1,center-12+j*9),new Vector3(2,5+(j%2)*3,3),dark,false,geometry);
                    Shape("Service Light",PrimitiveType.Cube,new Vector3(side*10.95f,1,center-12+j*9),new Vector3(.05f,.12f,2),cyan,false,geometry);
                }
            }
            for(int row=0;row<17;row++)
            {
                Shape("Deck Seam",PrimitiveType.Cube,new Vector3(0,.007f,center-16+row*2),new Vector3(15.5f,.012f,.025f),edge,false,geometry);
                Shape("Center Marker",PrimitiveType.Cube,new Vector3(0,.017f,center-16+row*2),new Vector3(.08f,.012f,.55f),white,false,geometry);
            }
            for(int x=-6;x<=6;x+=3) Shape("Longitudinal Seam",PrimitiveType.Cube,new Vector3(x,.008f,center),new Vector3(.022f,.01f,34),edge,false,geometry);
            WorldLabel("0"+(sector+1),new Vector3(-5.5f,.035f,center-13),2.5f,white);
            WorldLabel(sector==0?"CROSS TRAFFIC":sector==1?"ANGULAR MOMENTUM":"PULSE ARRAY",new Vector3(0,.035f,center-9),.38f,white);
        }
        Shape("Start Pad",PrimitiveType.Cube,new Vector3(0,.015f,3),new Vector3(4,.025f,3),edge,false);
        WorldLabel("START",new Vector3(0,.04f,1.7f),.55f,white);
        for(int i=0;i<4;i++)
        {
            float z=12+i*4.2f;
            Shape("Shuttle Lane",PrimitiveType.Cube,new Vector3(0,.025f,z),new Vector3(15,.025f,1.2f),dark,false);
            var obj=Shape("Cross Traffic "+i,PrimitiveType.Cube,new Vector3(0,.9f,z),new Vector3(3.7f,1.7f,.85f),coral,false);
            var collider=obj.GetComponent<Collider>(); collider.enabled=true; collider.isTrigger=true;
            var hazard=obj.AddComponent<CourseHazard>(); hazard.motion=HazardMotion.Shuttle; hazard.phase=i*1.7f; hazard.rate=1.05f+i*.12f; hazard.amplitude=5.1f;
            Shape("Hazard Light",PrimitiveType.Cube,new Vector3(0,1.77f,z),new Vector3(3.5f,.06f,.65f),gold,false).transform.SetParent(obj.transform,true);
        }
        Relay(0,31);
        for(int i=0;i<3;i++)
        {
            float z=42+i*8;
            Shape("Rotor Hub",PrimitiveType.Cylinder,new Vector3(i%2==0?-2:2,.7f,z),new Vector3(1.2f,.7f,1.2f),edge,true);
            var obj=Shape("Sweeper "+i,PrimitiveType.Cube,new Vector3(i%2==0?-2:2,.8f,z),new Vector3(9,.55f,.6f),coral,false);
            obj.GetComponent<Collider>().enabled=true; obj.GetComponent<Collider>().isTrigger=true;
            var hazard=obj.AddComponent<CourseHazard>(); hazard.motion=HazardMotion.Rotor; hazard.phase=i*1.3f; hazard.rate=i==1?-1.5f:1.2f;
        }
        Relay(1,65);
        for(int row=0;row<5;row++) for(int col=0;col<5;col++)
        {
            float x=-5.4f+col*2.7f, z=75+row*4;
            var warning=Shape("Pulse Warning Tile",PrimitiveType.Cube,new Vector3(x,.04f,z),new Vector3(2.4f,.035f,2.4f),gold,false);
            var obj=Shape("Pulse Pillar",PrimitiveType.Cube,new Vector3(x,1.3f,z),new Vector3(2.1f,2.6f,2.1f),coral,false);
            obj.GetComponent<Collider>().enabled=true; obj.GetComponent<Collider>().isTrigger=true;
            var hazard=obj.AddComponent<CourseHazard>(); hazard.motion=HazardMotion.Pulse; hazard.phase=(row+col)*.8f; hazard.rate=1; hazard.warning=warning.transform;
        }
        Relay(2,97);
        Cell(-5.8f,20); Cell(5.8f,49); Cell(-5.8f,86);
        Shape("Exit Deck",PrimitiveType.Cube,new Vector3(0,-.6f,106),new Vector3(16,1.2f,6),floor,true);
        for(int side=-1;side<=1;side+=2)
        {
            Shape("Exit Pillar",PrimitiveType.Cube,new Vector3(side*3,2.7f,106),new Vector3(.5f,5.4f,.7f),edge,true);
            Shape("Exit Strip",PrimitiveType.Cube,new Vector3(side*2.72f,2.7f,105.8f),new Vector3(.08f,5.2f,.08f),cyan,false);
        }
        Shape("Exit Lintel",PrimitiveType.Cube,new Vector3(0,5.4f,106),new Vector3(6.5f,.4f,.7f),cyan,false);
        WorldLabel("EXIT",new Vector3(0,.035f,104),.8f,white);
        var exit=new GameObject("Exit Trigger"); exit.transform.position=new Vector3(0,1.5f,106);
        var trigger=exit.AddComponent<BoxCollider>(); trigger.isTrigger=true; trigger.size=new Vector3(5,3,2);
        exit.AddComponent<CoursePickup>().kind=PickupKind.Exit;
        Shape("End Barrier",PrimitiveType.Cube,new Vector3(0,1,109),new Vector3(16,2,.3f),edge,true);

        PlayerSettings.productName="Relay Run";
        PlayerSettings.companyName="PA2 Portfolio";
        PlayerSettings.defaultScreenWidth=1600; PlayerSettings.defaultScreenHeight=900;
        PlayerSettings.fullScreenMode=FullScreenMode.Windowed;
        PlayerSettings.runInBackground=true;
        QualitySettings.antiAliasing=4; QualitySettings.vSyncCount=1;
        EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene(),ScenePath);
        EditorBuildSettings.scenes=new[] { new EditorBuildSettingsScene(ScenePath,true),new EditorBuildSettingsScene("Assets/Scenes/start.unity",false),new EditorBuildSettingsScene("Assets/Scenes/main.unity",false),new EditorBuildSettingsScene("Assets/Scenes/Over.unity",false),new EditorBuildSettingsScene("Assets/Scenes/Clear.unity",false) };
        AssetDatabase.SaveAssets();
        Debug.Log("PORTFOLIO_SCENE_CREATED");
    }
    static Material Mat(string name,Color color,float emission=0)
    {
        string path=Root+"/Materials/"+name+".mat";
        var mat=AssetDatabase.LoadAssetAtPath<Material>(path);
        if(!mat) { mat=new Material(Shader.Find("Standard")); AssetDatabase.CreateAsset(mat,path); }
        mat.color=color; mat.SetFloat("_Glossiness",.35f); mat.SetFloat("_Metallic",.3f);
        if(emission>0) { mat.EnableKeyword("_EMISSION"); mat.SetColor("_EmissionColor",color*emission); }
        EditorUtility.SetDirty(mat); return mat;
    }
    static GameObject Shape(string name,PrimitiveType type,Vector3 position,Vector3 scale,Material material,bool solid,Transform parent=null)
    {
        var obj=GameObject.CreatePrimitive(type); obj.name=name; obj.transform.position=position; obj.transform.localScale=scale;
        obj.GetComponent<Renderer>().sharedMaterial=material; obj.GetComponent<Collider>().enabled=solid;
        if(parent) obj.transform.SetParent(parent,true); return obj;
    }
    static void WorldLabel(string text,Vector3 position,float size,Material material)
    {
        var obj=new GameObject(text); obj.transform.position=position; obj.transform.rotation=Quaternion.Euler(90,0,0);
        var mesh=obj.AddComponent<TextMesh>(); mesh.text=text; mesh.fontSize=64; mesh.characterSize=size*.12f; mesh.anchor=TextAnchor.MiddleCenter; mesh.alignment=TextAlignment.Center; mesh.color=material.color;
    }
    static void Relay(int index,float z)
    {
        Shape("Checkpoint Pad",PrimitiveType.Cube,new Vector3(0,.022f,z),new Vector3(6,.04f,3.4f),edge,false);
        Shape("Checkpoint Stripe",PrimitiveType.Cube,new Vector3(0,.05f,z-1.6f),new Vector3(6,.04f,.07f),cyan,false);
        var obj=new GameObject("Relay "+(index+1)); obj.transform.position=new Vector3(0,0,z);
        var trigger=obj.AddComponent<BoxCollider>(); trigger.isTrigger=true; trigger.center=Vector3.up; trigger.size=new Vector3(4,3,3);
        var pickup=obj.AddComponent<CoursePickup>(); pickup.kind=PickupKind.Relay; pickup.index=index; pickup.checkpoint=new Vector3(0,.1f,z+1);
        var gem=Shape("Relay Core",PrimitiveType.Cube,new Vector3(0,1.8f,z),Vector3.one*.75f,cyan,false); gem.transform.rotation=Quaternion.Euler(30,0,45); gem.transform.SetParent(obj.transform,true); pickup.visual=gem.transform;
        WorldLabel("RELAY 0"+(index+1),new Vector3(0,.06f,z-1),.38f,white);
    }
    static void Cell(float x,float z)
    {
        var obj=new GameObject("Energy Cell"); obj.transform.position=new Vector3(x,0,z);
        var collider=obj.AddComponent<SphereCollider>(); collider.isTrigger=true; collider.center=Vector3.up; collider.radius=1.1f;
        var pickup=obj.AddComponent<CoursePickup>(); pickup.kind=PickupKind.Cell;
        var gem=Shape("Cell Visual",PrimitiveType.Sphere,new Vector3(x,1.1f,z),Vector3.one*.65f,cyan,false); gem.transform.SetParent(obj.transform,true); pickup.visual=gem.transform;
        Shape("Cell Pad",PrimitiveType.Cylinder,new Vector3(x,.04f,z),new Vector3(1.5f,.035f,1.5f),edge,false);
    }
}

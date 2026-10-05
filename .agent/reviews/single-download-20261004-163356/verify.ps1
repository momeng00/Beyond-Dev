$ErrorActionPreference = 'Stop'
$repo = (Resolve-Path (Join-Path $PSScriptRoot '../../..')).Path
$enc = [Text.Encoding]::GetEncoding(28591)
$sources = @(
    'Assets/02.Script/Object/Spot/UploadStation.cs',
    'Assets/02.Script/Object/Spot/DownloadStation.cs'
) | ForEach-Object { $enc.GetString([IO.File]::ReadAllBytes((Join-Path $repo $_))) }
$harness = @'
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
namespace Unity.VisualScripting { public class Placeholder {} }
namespace UnityEngine {
 public class Object {
  public bool destroyed;
  public static bool operator ==(Object a,Object b) { bool an=ReferenceEquals(a,null)||a.destroyed,bn=ReferenceEquals(b,null)||b.destroyed; return an||bn?an==bn:ReferenceEquals(a,b); }
  public static bool operator !=(Object a,Object b) {return !(a==b);}
  public override bool Equals(object o){return ReferenceEquals(this,o);}
  public override int GetHashCode(){return System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(this);}
  public static void Destroy(Object obj){if(!ReferenceEquals(obj,null))obj.destroyed=true;}
  public static GameObject Instantiate(GameObject original){return original.Copy();}
 }
 public class SerializeField:Attribute {}
 public class Component:Object {
  public GameObject gameObject;
  public Transform transform {get{return gameObject.transform;}}
  public T GetComponent<T>() where T:class {return gameObject.GetComponent<T>();}
  public T GetComponentInChildren<T>() where T:class {return GetComponent<T>();}
  public bool TryGetComponent<T>(out T value) where T:class {return gameObject.TryGetComponent(out value);}
 }
 public class MonoBehaviour:Component {}
 public class GameObject:Object {
  public bool activeSelf=true;public Transform transform=new Transform();public int layer;
  private List<Component> components=new List<Component>();
  public T AddComponent<T>() where T:Component,new(){var value=new T();value.gameObject=this;components.Add(value);return value;}
  public T GetComponent<T>() where T:class {foreach(var c in components)if(c is T)return c as T;return null;}
  public bool TryGetComponent<T>(out T value) where T:class {value=GetComponent<T>();return value!=null;}
  public void SetActive(bool value){activeSelf=value;}
  public GameObject Copy(){var copy=new GameObject();copy.transform.position=transform.position;
   copy.AddComponent<Rigidbody2D>();copy.AddComponent<Collider2D>();copy.AddComponent<SpriteRenderer>();
   if(GetComponent<PushBlock>()!=null)copy.AddComponent<PushBlock>();else copy.AddComponent<Block>();return copy;}
 }
 public class Transform {public Vector3 position;}
 public struct Vector2 {public float x,y;public Vector2(float x,float y){this.x=x;this.y=y;}public static Vector2 zero {get{return new Vector2();}}public static implicit operator Vector2(Vector3 v){return new Vector2(v.x,v.y);}}
 public struct Vector3 {public float x,y,z;public Vector3(float x,float y,float z){this.x=x;this.y=y;this.z=z;}public static Vector3 operator -(Vector3 a,Vector3 b){return new Vector3(a.x-b.x,a.y-b.y,a.z-b.z);}}
 public class Bounds {public Vector3 min,max;public bool Contains(Vector3 x){return true;}}
 public class Collider2D:Component {public bool enabled=true;public Bounds bounds=new Bounds();}
 public class BoxCollider2D:Collider2D {public bool isTrigger;public Vector2 size,offset;}
 public enum RigidbodyType2D {Dynamic,Kinematic,Static}
 public class Rigidbody2D:Component {public RigidbodyType2D bodyType;public Vector2 linearVelocity;public float angularVelocity;}
 public class Material {public void SetVector(string key,Vector2 value){}}
 public class SpriteRenderer:Component {public Material material=new Material();public Vector2 size=new Vector2(10,10);public bool enabled=true;}
 public class Animator:Component {public Dictionary<string,bool> values=new Dictionary<string,bool>();public void SetBool(string key,bool value){values[key]=value;}}
 public static class Debug {public static void Log(object value){}}
}
public interface ISwitchable {Switch Switch{get;}bool SwitchOn(bool value);}
public interface IReset {void InitializeReset();void ResetAction();}
public class Spot:MonoBehaviour {protected int layerMask;protected bool IsInLayerMask(GameObject obj,int mask){return true;}}
public class Switch:MonoBehaviour {public void SetSwitch(ISwitchable value){}}
public class DownloadStationSwtich:Switch {public bool isUpload;public Animator anime=new Animator();}
public class Block:MonoBehaviour {public void OnBlockAction(){}}
public class PushBlock:Block {public List<bool> calls=new List<bool>();public void UDAnimationPlay(bool value){calls.Add(value);gameObject.SetActive(true);}}
public class GameManager {public static GameManager Instance=new GameManager();public Action OnReset;}
public static class StationChecks {
 static int checks;
 static void Check(bool result,string label){checks++;if(!result)throw new Exception(label);}
 static BindingFlags flags=BindingFlags.Instance|BindingFlags.NonPublic;
 static void Set(object obj,string field,object value){obj.GetType().GetField(field,flags).SetValue(obj,value);}
 static T Get<T>(object obj,string field){return (T)obj.GetType().GetField(field,flags).GetValue(obj);}
 static void Call(object obj,string method){obj.GetType().GetMethod(method,flags).Invoke(obj,null);}
 static GameObject BlockObject(bool push=false){var go=new GameObject();go.AddComponent<Rigidbody2D>();go.AddComponent<Collider2D>();go.AddComponent<SpriteRenderer>();if(push)go.AddComponent<PushBlock>();else go.AddComponent<Block>();go.transform.position=new Vector3(2,3,0);return go;}
 static DownloadStation Station(UploadStation owner,float x){var go=new GameObject();go.transform.position=new Vector3(x,20,0);go.AddComponent<SpriteRenderer>();go.AddComponent<Animator>();var ds=go.AddComponent<DownloadStation>();ds.switches=new List<DownloadStationSwtich>{new DownloadStationSwtich()};Set(ds,"partnerStation",owner);Call(ds,"Awake");return ds;}
 static void Detect(UploadStation up,GameObject ob){Get<List<GameObject>>(up,"detectedList").Add(ob);}
 static int CacheCount(UploadStation up){return ((System.Collections.IDictionary)up.GetType().GetField("readyList",flags).GetValue(up)).Count;}
 public static int Run(){
  var go=new GameObject();go.AddComponent<SpriteRenderer>();go.AddComponent<BoxCollider2D>();go.AddComponent<Animator>();
  var up=go.AddComponent<UploadStation>();up.switches=new List<Switch>();Call(up,"Awake");
  var a=Station(up,100);var b=Station(up,200);up.partnerStations.Add(a);up.partnerStations.Add(b);Call(up,"Start");
  var original=BlockObject();
  Check(!a.SwitchOn(true),"download before upload");
  Check(!up.SwitchOn(true),"empty upload");
  Detect(up,original);Check(up.SwitchOn(true),"upload accepted");
  var ac=a.blocks[0];var bc=b.blocks[0];
  Check(ac!=bc&&CacheCount(up)==2,"separate preview clones");
  Check(ac.transform.position.x==102&&bc.transform.position.x==202,"preview positions");
  Check(!ac.GetComponent<Collider2D>().enabled&&!bc.GetComponent<Collider2D>().enabled,"preview colliders disabled");
  Check(ac.GetComponent<Rigidbody2D>().bodyType==RigidbodyType2D.Kinematic&&bc.GetComponent<Rigidbody2D>().bodyType==RigidbodyType2D.Kinematic,"preview bodies");
  Check(!a.SwitchOn(false),"false is not download");
  Check(a.SwitchOn(true),"first selected");
  Check(ac.GetComponent<Collider2D>().enabled&&ac.GetComponent<Rigidbody2D>().bodyType==RigidbodyType2D.Dynamic,"selected physical activation");
  Check(!b.SwitchOn(true)&&!a.SwitchOn(true),"other/repeated denied");
  Check(!bc.GetComponent<Collider2D>().enabled,"other preview not physical");
  Check(!a.switches[0].isUpload&&!b.switches[0].isUpload,"input switches locked");
  Check(!up.SwitchOn(true),"reupload cannot unlock selection");
  ac.transform.position=new Vector3(99,99,0);up.invisibleBlock();Check(ac.transform.position.x==99,"preview cannot relocate download");
  Get<List<GameObject>>(up,"detectedList").Clear();Check(up.SwitchOn(false),"recall without detected objects");
  Check(original.GetComponent<Rigidbody2D>().bodyType==RigidbodyType2D.Dynamic&&original.GetComponent<Collider2D>().enabled&&original.GetComponent<SpriteRenderer>().enabled,"original restored");
  Check(!ac.activeSelf&&!bc.activeSelf&&a.blocks.Count==0&&b.blocks.Count==0,"all generic clones returned");
  Check(!b.SwitchOn(true),"no download until new upload");
  Detect(up,original);ac.GetComponent<Rigidbody2D>().linearVelocity=new Vector2(5,8);ac.GetComponent<Rigidbody2D>().angularVelocity=4;
  Check(up.SwitchOn(true),"new upload");
  Check(a.blocks[0]==ac&&b.blocks[0]==bc&&CacheCount(up)==2,"pool reuse");
  Check(ac.GetComponent<Rigidbody2D>().linearVelocity.x==0&&ac.GetComponent<Rigidbody2D>().angularVelocity==0,"pool velocity reset");
  Check(b.SwitchOn(true),"different station after recall");
  up.PoolingReturn();Detect(up,BlockObject());Check(up.SwitchOn(true),"two originals");
  Check(a.blocks.Count==2&&b.blocks.Count==2&&CacheCount(up)==4,"two by two previews");
  up.GetDetectedObject(a.gameObject);Check(ac.GetComponent<Rigidbody2D>().bodyType==RigidbodyType2D.Dynamic,"legacy entry selects");
  up.GetDetectedObject(b.gameObject);Check(bc.GetComponent<Rigidbody2D>().bodyType==RigidbodyType2D.Kinematic,"legacy entry cannot bypass lock");
  up.ResetAction();Check(ac.destroyed&&bc.destroyed&&CacheCount(up)==0,"reset cache destroyed");
  Check(a.blocks.Count==0&&b.blocks.Count==0&&!b.SwitchOn(true),"reset clears availability");
  Detect(up,original);Check(up.SwitchOn(true)&&b.SwitchOn(true),"selection after reset");
  UnityEngine.Object.Destroy(b);Check(!a.SwitchOn(true),"destroyed selected station remains locked");
  up.ResetAction();Check(CacheCount(up)==0,"reset with destroyed station");
  Detect(up,original);Check(up.SwitchOn(true)&&a.SwitchOn(true),"remaining station after reset");
  up.PoolingReturn();Get<List<GameObject>>(up,"detectedList").Clear();Detect(up,BlockObject(true));Check(up.SwitchOn(true),"push upload");
  var pushClone=a.blocks[0];up.PoolingReturn();
  Check(pushClone.GetComponent<PushBlock>().calls.Contains(false)&&!pushClone.GetComponent<Collider2D>().enabled,"push recall animation and physics");
  up.ResetAction();Check(pushClone.destroyed,"cached push clone destroyed after prior recall");
  var unlinked=Station(null,300);Check(!unlinked.SwitchOn(true),"missing partner denied");
  var outsider=Station(up,400);Detect(up,original);up.SwitchOn(true);Check(!outsider.SwitchOn(true),"nonpartner denied");
  var lost=a.blocks[0];UnityEngine.Object.Destroy(lost);Check(!a.SwitchOn(true),"destroyed preview denied");
  up.PoolingReturn();Check(up.SwitchOn(true)&&a.blocks[0]!=lost,"destroyed cached clone recreated");
  up.ResetAction();up.ResetAction();Check(CacheCount(up)==0,"repeated reset");
  return checks;
 }
}
'@
$compiledSources = $sources | ForEach-Object { [regex]::Replace($_, '(?m)^using [^;]+;\r?\n', '') }
Add-Type -TypeDefinition ($harness + "`r`n" + ($compiledSources -join "`r`n")) -IgnoreWarnings -WarningAction SilentlyContinue
"Passed $([StationChecks]::Run()) checks using full UploadStation/DownloadStation sources and Unity/dependency substitutes. Not Unity Play Mode."

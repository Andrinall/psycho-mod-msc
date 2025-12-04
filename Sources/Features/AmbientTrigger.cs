
using UnityEngine;

using Psycho.Internal;


namespace Psycho.Features
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(Collider))]
    class AmbientTrigger : CatchedComponent
    {
        public AudioSource LocalAmbientSource;
        public bool CheckTimeOfDay = false;

        GameObject jonnez;
        GameObject boat;

        Collider selfColl;

        Vector3 ClosestPoint => selfColl.ClosestPointOnBounds(Globals.Player.position);

        protected override void Awaked()
        {
            var _source = GetComponent<AudioSource>();
            if (_source != null)
                LocalAmbientSource = _source;

            jonnez = GameObject.Find("JONNEZ ES(Clone)");
            boat = GameObject.Find("BOAT/GFX/Colliders/Collider");

            selfColl = GetComponent<Collider>();
        }

        protected override void OnFixedUpdate()
        {
            if (Globals.Player == null || jonnez == null || boat == null || selfColl == null)
            {
                Destroy(this);
                throw new System.NullReferenceException($"AmbientTrigger handler disabled by nullcheck in [ {transform.GetPath()} ( AmbientTrigger.OnFixedUpdate() ) ]");
            }

            if (Vector3.Distance(ClosestPoint, Globals.Player.position) == 0)
            {
                Logic.CurrentAmbientTrigger = this;
                if (!CheckTimeOfDay)
                {
                    MuteAmbient(false);
                    return;
                }

                if (Globals.SUN_Hours >= 22f || Globals.SUN_Hours < 4f)
                {
                    MuteAmbient(false);
                    return;
                }

                MuteAmbient(true);
                return;
            }
            
            
            if (Logic.CurrentAmbientTrigger != this) return;
                
            MuteAmbient(true);
            Logic.CurrentAmbientTrigger = null;
        }

        public void MuteAmbient(bool state)
        {
            MuteSource(LocalAmbientSource, state);
            MuteGlobalAmbient(!state);

        }

        bool _isNeededObject(Collider other)
        {
            GameObject _obj = other?.gameObject;
            if (_obj == null) return false;
            return _obj == Globals.Player.gameObject || _obj == jonnez || _obj == boat;
        }


        public static void InitializeAllTriggers()
        {
            GameObject _houseTriggerObj = new GameObject("HouseAmbientTrigger");
            _houseTriggerObj.transform.position = new Vector3(-6.67f, 0.7f, 9.47f);
            SoundManager.AddAudioSource(_houseTriggerObj, ResourcesStorage.HouseAmbient_clip, 0.22f);

            BoxCollider _box = _houseTriggerObj.AddComponent<BoxCollider>();
            _box.isTrigger = true;
            _box.center = new Vector3(-0.4707041f, 0.172927f, 0.4841557f);
            _box.size = new Vector3(11.49759f, 2.885854f, 10.05963f);

            _houseTriggerObj.AddComponent<AmbientTrigger>().CheckTimeOfDay = true;

            GameObject _houseTriggerObj2 = new GameObject("HouseAmbientTrigger2");
            _houseTriggerObj2.transform.position = new Vector3(-6.67f, 0.7f, -0.6f);

            BoxCollider _box2 = _houseTriggerObj2.AddComponent<BoxCollider>();
            _box2.isTrigger = true;
            _box2.center = new Vector3(-5.927717f, -0.1420171f, 1.617358f);
            _box2.size = new Vector3(2.983747f, 2.255966f, 7.793219f);

            AmbientTrigger _trigger = _houseTriggerObj2.AddComponent<AmbientTrigger>();
            _trigger.LocalAmbientSource = _houseTriggerObj.GetComponent<AudioSource>();
            _trigger.CheckTimeOfDay = true;


            GameObject _islandAmbientObj = new GameObject("IslandAmbientTrigger");
            _islandAmbientObj.transform.position = new Vector3(-878.7f, -3.695f, 496.6f);
            SoundManager.AddAudioSource(_islandAmbientObj, ResourcesStorage.IslandAmbient_clip, 0.26f);

            SphereCollider _sphere = _islandAmbientObj.AddComponent<SphereCollider>();
            _sphere.isTrigger = true;
            _sphere.center = new Vector3(-0.4707041f, 0.172927f, 0.4841557f);
            _sphere.radius = 70f;

            _islandAmbientObj.AddComponent<AmbientTrigger>();


            GameObject _dingonbiisiAmbientObj = new GameObject("DingonbiisiAmbientTrigger");
            _dingonbiisiAmbientObj.transform.position = new Vector3(1368.03f, 10.63f, 799.7194f);
            _dingonbiisiAmbientObj.transform.eulerAngles = new Vector3(0f, 39.213f, 0f);
            SoundManager.AddAudioSource(_dingonbiisiAmbientObj, ResourcesStorage.DingonbiisiAmbient_clip, 0.09f);

            BoxCollider _box3 = _dingonbiisiAmbientObj.AddComponent<BoxCollider>();
            _box3.isTrigger = true;
            _box3.center = new Vector3(-0.04533959f, 0.3857429f, -0.001901387f);
            _box3.size = new Vector3(18.84068f, 7.897154f, 5.921038f);

            _dingonbiisiAmbientObj.AddComponent<AmbientTrigger>();
        }

        public static void MuteGlobalAmbient(bool state)
        {
            if (state)
            {
                MuteSource(Globals.GlobalAmbient_source, true);
                MuteSource(Globals.GlobalPsychoAmbient_source, true);
                return;
            }

            if (!Logic.InHorror)
            {
                MuteSource(Globals.GlobalAmbient_source, !(Globals.SUN_Hours >= 22f || Globals.SUN_Hours < 4f));
                MuteSource(Globals.GlobalPsychoAmbient_source, true);
                return;
            }
            
            MuteSource(Globals.GlobalAmbient_source, true);
            MuteSource(Globals.GlobalPsychoAmbient_source, false);
        }

        public static void MuteSource(AudioSource source, bool state)
        {
            if (source.mute == state) return;
            source.mute = state;
        }
    }
}

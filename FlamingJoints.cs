namespace DestructionEffects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using UnityEngine;

    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class FlamingJoints : MonoBehaviour
    {
        public static List<GameObject> FlameObjects = new List<GameObject>();

        public static List<string> PartTypesTriggeringUnwantedJointBreakEvents = new List<string>(8)
                                                                                 {
                                                                                     "decoupler",
                                                                                     "separator",
                                                                                     "docking",
                                                                                     "grappling",
                                                                                     "landingleg",
                                                                                     "clamp",
                                                                                     "gear",
                                                                                     "wheel"
                                                                                 };
        public void Start()
        {
            GameEvents.onPartJointBreak.Add(this.OnPartJointBreak);     
        }

        public void OnPartJointBreak(PartJoint partJoint)
        {
            if (partJoint.Target == null)
            {
                return;
            }
            if (partJoint.Target.PhysicsSignificance == 1)
            {
                return;
            }
            if (!ShouldFlamesBeAttached(partJoint))
            {
                return;
            }

            AttachFlames(partJoint);
        }

        private static void AttachFlames(PartJoint partJoint)
        {
            var flameObject2 =
                (GameObject)
                Instantiate(
                    GameDatabase.Instance.GetModel("DestructionEffects/Models/FlameEffect/model"),
                    partJoint.transform.position,
                    Quaternion.identity);

            flameObject2.SetActive(true);
            flameObject2.transform.parent = partJoint.Target.transform;
            flameObject2.AddComponent<FlamingJointScript>();

            foreach (var pe in flameObject2.GetComponentsInChildren<KSPParticleEmitter>())
            {
                if (!pe.useWorldSpace) continue;
                var gpe = pe.gameObject.AddComponent<DeGaplessParticleEmitter>();
                gpe.Part = partJoint.Target;
                gpe.Emit = true;
            }
        }

        private static bool ShouldFlamesBeAttached(PartJoint partJoint)
        {       
            if (IsPartHostTypeAJointBreakerTrigger(partJoint.Host.name.ToLower()))
            {
                return false;
            }
            var part = partJoint.Target;
            if (part.partInfo.title.Contains("Wing") || part.partInfo.title.Contains("Fuselage")
                || part.FindModuleImplementing<ModuleEngines>() || part.FindModuleImplementing<ModuleEnginesFX>())
            {
                return true;
            }

            return part.Resources.Cast<PartResource>().Any(resource => resource.resourceName.Contains("Fuel") || resource.resourceName.Contains("Ox"));
        }

        private static bool IsPartHostTypeAJointBreakerTrigger(string hostPartName)
        {

            return PartTypesTriggeringUnwantedJointBreakEvents.Any(x => hostPartName.Contains(x));
        }


    }
}
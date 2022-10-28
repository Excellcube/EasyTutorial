using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Excellcube.EasyTutorial.Utils
{
    public class TutorialEvent {
        public delegate void EventReceived();

        static private TutorialEvent sInstance = null;
        static public  TutorialEvent Instance {
            get {
                if(sInstance == null) {
                    sInstance = new TutorialEvent();
                }
                return sInstance;
            }
            private set {}
        }

        private Dictionary<string, Dictionary<object, EventReceived>> m_EventMap 
            = new Dictionary<string, Dictionary<object, EventReceived>>();
        private Dictionary<object, List<string>> m_TargetMap 
            = new Dictionary<object, List<string>>();
        
        public void Listen(string eventId, object target, EventReceived callback) {
            if(!m_EventMap.ContainsKey(eventId)) {
                m_EventMap.Add(eventId, new Dictionary<object, EventReceived>());
            }
            var map = m_EventMap[eventId];
            if(!map.ContainsKey(target)) {
                map.Add(target, callback);
                if(!m_TargetMap.ContainsKey(target)) {
                    m_TargetMap.Add(target, new List<string>());
                }
                m_TargetMap[target].Add(eventId);
            }
        }

        public void Broadcast(string eventId) {
            if(m_EventMap.ContainsKey(eventId)) {
                Dictionary<object, EventReceived> map = m_EventMap[eventId];
                foreach(KeyValuePair<object, EventReceived> p in map) {
                    p.Value();
                }
            }
        }

        public void Unlisten(string eventId, object target)
        {
            if (m_EventMap.ContainsKey(eventId))
            {
                m_EventMap[eventId].Remove(target);
                if (m_TargetMap.ContainsKey(target))
                {
                    m_TargetMap[target].Remove(eventId);
                }
            }
            else
            {
                Debug.LogWarningFormat("TutorialEvent doesn't have a key {0}", eventId);
            }
        }

        public void UnlistenAll()
        {
            m_EventMap.Clear();
            m_TargetMap.Clear();
        }
        
        public void UnlistenAll(string eventId) 
        {
            if (m_EventMap.ContainsKey(eventId))
            {
                Dictionary<object, EventReceived> eventMap = m_EventMap[eventId];
                foreach (var elem in eventMap)
                {
                    m_TargetMap[elem.Key].Remove(eventId);
                }
                m_EventMap.Remove(eventId);
            }
        }

        public void UnlistenAll(Object target) {
            foreach(KeyValuePair<object, List<string>> p in m_TargetMap) {
                if(p.Key.Equals(target)) {
                    List<string> eventIds = p.Value;
                    foreach(string eventId in eventIds.ToArray()) {
                        Unlisten(eventId, p.Key);
                    }
                }
            }
        }
    }
}
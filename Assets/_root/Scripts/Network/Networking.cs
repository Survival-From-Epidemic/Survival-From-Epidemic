using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Networking;

namespace _root.Scripts.Network
{
    public class Networking : MonoBehaviour
    {
        private static string _baseUrl;
        private static int _timeOut;
        private static Networking _networking;

        [SerializeField]
        private string baseUrl;
        [SerializeField]
        private int timeOut = 30;

        private void Start()
        {
            _baseUrl = baseUrl;
            _timeOut = timeOut;
            if (_networking != null)
                Destroy(_networking);
            _networking = this;
        }

        public abstract class Request<T> where T : class
        {
            private readonly LinkedList<string> _params;
            private readonly Dictionary<string, string> _headers;

            private readonly string _path;

            [CanBeNull]
            private Action _errorAction;
            [CanBeNull]
            private Action<T> _responseAction;

            protected Request(string path)
            {
                _params = new LinkedList<string>();
                _headers = new Dictionary<string, string>();
                _path = path;
            }

            public Request<T> AddParam(string key, string value)
            {
                _params.AddLast($"{key}={HttpUtility.UrlEncode(value)}");
                return this;
            }

            public Request<T> AddHeader(string key, string value)
            {
                _headers[key] = value;
                return this;
            }

            public Request<T> OnError(Action action)
            {
                _errorAction = action;
                return this;
            }

            public Request<T> OnResponse(Action<T> action)
            {
                _responseAction = action;
                return this;
            }

            protected abstract UnityWebRequest WebRequest(string url);

            private IEnumerator _Request(string url)
            {
                using var webRequest = UnityWebRequest.Get(url);
                webRequest.timeout = _timeOut;
                foreach (var (key, value) in _headers)
                    webRequest.SetRequestHeader(key, value);
                yield return webRequest.SendWebRequest();

                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    var text = webRequest.downloadHandler.text;
                    if (typeof(T) == typeof(string)) {
                        _responseAction?.Invoke(text as T);
                    } else {
                        _responseAction?.Invoke(JsonUtility.FromJson<T>(webRequest.downloadHandler.text));
                    }
                    yield break;
                }

                _errorAction?.Invoke();
            }

            public void Build()
            {
                var parameters = $"?{string.Join("&", _params)}";
                _networking.StartCoroutine(_Request(_baseUrl + _path + parameters));
            }
        }

        public class Get<T> : Request<T> where T : class
        {
            public Get(string path) : base(path) { }

            protected override UnityWebRequest WebRequest(string url) => UnityWebRequest.Get(url);
        }

        public class Post<T> : Request<T> where T : class
        {
            private readonly string _body;

            public Post(string path, object body) : base(path) => _body = JsonUtility.ToJson(body);

            protected override UnityWebRequest WebRequest(string url) =>
                UnityWebRequest.PostWwwForm(url, _body);
        }

        public class Put<T> : Request<T> where T : class
        {
            private readonly string _body;

            public Put(string path, object body) : base(path) => _body = JsonUtility.ToJson(body);

            protected override UnityWebRequest WebRequest(string url) =>
                UnityWebRequest.Put(url, _body);
        }

        public class Delete<T> : Request<T> where T : class
        {
            public Delete(string path) : base(path) { }

            protected override UnityWebRequest WebRequest(string url) =>
                UnityWebRequest.Delete(url);
        }
    }
}

using System;
using UnityEngine;

namespace Claws.Modifiers
{
    internal class ClawTrail : MonoBehaviour
    {
        SaberTrailRenderer _trailRendererPrefab;

        SaberTrailRenderer _middleClawRenderer, _leftClawRenderer, _rightClawRenderer;

        float _trailDuration = 0.4f;
        int _samplingFrequency = 50;
        int _granularity = 60;
        float _whiteSectionMaxDuration = 0.1f;
        Color _color;
        IBladeMovementData _movementData;
        float _lastTrailElementTime;
        TrailElementCollection _trailElementCollection;
        float _sampleStep;
        int _framesPassed;
        float _lastZScale;
        int _framesToScaleCheck;
        bool _inited;

        const float TrailYOffset = 0.0075f;
        const float TrailXOffset = 0.025f;
        static float TrailLength => Preferences.Length * 0.75f;

        public void RegisterPrefab(SaberTrailRenderer prefab)
        {
            _trailRendererPrefab = prefab;
        }

        public void Setup(Color color, IBladeMovementData movementData)
        {
            _color = color;
            _movementData = movementData;
        }

        public void Init()
        {
            if (!_middleClawRenderer)
                _middleClawRenderer = Instantiate(_trailRendererPrefab, new Vector3(0, TrailYOffset), Quaternion.identity);
            if (!_leftClawRenderer)
                _leftClawRenderer = Instantiate(_trailRendererPrefab, new Vector3(-TrailXOffset, TrailYOffset), Quaternion.identity);
            if (!_rightClawRenderer)
                _rightClawRenderer = Instantiate(_trailRendererPrefab, new Vector3(TrailXOffset, TrailYOffset), Quaternion.identity);

            _sampleStep = 1f / _samplingFrequency;
            var lastAddedData = _movementData.lastAddedData;
            var bottomPos = lastAddedData.bottomPos;
            var topPos = lastAddedData.topPos;
            _lastTrailElementTime = lastAddedData.time;
            _trailElementCollection = new TrailElementCollection(Mathf.CeilToInt(_samplingFrequency * _trailDuration) + 3, bottomPos, CalcNewTopPos(bottomPos, topPos), _lastTrailElementTime);
            var trailWidth = GetTrailWidth(lastAddedData);
            _whiteSectionMaxDuration = Math.Min(_whiteSectionMaxDuration, _trailDuration);
            _lastZScale = transform.lossyScale.z;
            _middleClawRenderer.Init(trailWidth, _trailDuration, _granularity, _whiteSectionMaxDuration);
            _leftClawRenderer.Init(trailWidth, _trailDuration * 0.65f, _granularity, _whiteSectionMaxDuration * 0.65f);
            _rightClawRenderer.Init(trailWidth, _trailDuration * 0.65f, _granularity, _whiteSectionMaxDuration * 0.65f);
            _inited = true;
        }

        public void LateUpdate()
        {
            if (_framesPassed <= 4)
            {
                if (_framesPassed == 4)
                    Init();
                ++_framesPassed;
            }
            else
            {
                var lastAddedData = _movementData.lastAddedData;
                var prevAddedData = _movementData.prevAddedData;
                --_framesToScaleCheck;
                if (_framesToScaleCheck <= 0)
                {
                    _framesToScaleCheck = 10;
                    if (!Mathf.Approximately(transform.lossyScale.z, _lastZScale))
                    {
                        _lastZScale = transform.lossyScale.z;
                        _middleClawRenderer.SetTrailWidth(GetTrailWidth(lastAddedData));
                    }
                }
                var numSamples = (int)Mathf.Floor((lastAddedData.time - _lastTrailElementTime) / _sampleStep);
                for (var index = 0; index < numSamples; ++index)
                {
                    _lastTrailElementTime += _sampleStep;
                    var t = (lastAddedData.time - _lastTrailElementTime) / (lastAddedData.time - prevAddedData.time);

                    var bottom = Vector3.LerpUnclamped(lastAddedData.bottomPos, prevAddedData.bottomPos, t);
                    var top = Vector3.LerpUnclamped(lastAddedData.topPos, prevAddedData.topPos, t);

                    _trailElementCollection.SetHeadData(bottom, CalcNewTopPos(bottom, top), _lastTrailElementTime);
                    _trailElementCollection.MoveTailToHead();
                }
                _trailElementCollection.SetHeadData(lastAddedData.bottomPos, lastAddedData.topPos, lastAddedData.time);
                _trailElementCollection.UpdateDistances();

                _middleClawRenderer.transform.position = transform.rotation * new Vector3(0, TrailYOffset);
                _leftClawRenderer.transform.position = transform.rotation * new Vector3(-TrailXOffset, TrailYOffset);
                _rightClawRenderer.transform.position = transform.rotation * new Vector3(TrailXOffset, TrailYOffset);

                _middleClawRenderer.UpdateMesh(_trailElementCollection, _color);
                _leftClawRenderer.UpdateMesh(_trailElementCollection, _color);
                _rightClawRenderer.UpdateMesh(_trailElementCollection, _color);
            }
        }

        Vector3 CalcNewTopPos(Vector3 bottom, Vector3 top)
        {
            return bottom + (top - bottom).normalized * TrailLength;
        }

        public void OnEnable()
        {
            if (_inited)
            {
                ResetTrailData();
                _middleClawRenderer.UpdateMesh(_trailElementCollection, _color);
                _leftClawRenderer.UpdateMesh(_trailElementCollection, _color);
                _rightClawRenderer.UpdateMesh(_trailElementCollection, _color);
            }
            if (_middleClawRenderer)
                _middleClawRenderer.enabled = true;
            if (_leftClawRenderer)
                _leftClawRenderer.enabled = true;
            if (_rightClawRenderer)
                _rightClawRenderer.enabled = true;
        }

        public void OnDisable()
        {
            if (_middleClawRenderer)
                _middleClawRenderer.enabled = false;
            if (_leftClawRenderer)
                _leftClawRenderer.enabled = false;
            if (_rightClawRenderer)
                _rightClawRenderer.enabled = false;
        }

        public void OnDestroy()
        {
            if (_middleClawRenderer)
                Destroy(_middleClawRenderer.gameObject);
            if (_leftClawRenderer)
                Destroy(_leftClawRenderer.gameObject);
            if (_rightClawRenderer)
                Destroy(_rightClawRenderer.gameObject);
        }

        public void ResetTrailData()
        {
            var lastAddedData = _movementData.lastAddedData;
            var bottomPos = lastAddedData.bottomPos;
            var topPos = lastAddedData.topPos;
            _lastTrailElementTime = lastAddedData.time;
            _trailElementCollection.InitSnapshots(bottomPos, CalcNewTopPos(bottomPos, topPos), _lastTrailElementTime);
        }

        public float GetTrailWidth(BladeMovementDataElement lastAddedData) => TrailLength;
    }
}
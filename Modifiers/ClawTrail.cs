using IPA.Utilities;
using System;
using UnityEngine;

namespace Claws.Modifiers
{
    internal static class TrailElementCollectionExtensions
    {
        static readonly AlmostVersion NewTrailApiVersion = new AlmostVersion("1.19.1");

        static FieldAccessor<TrailElementCollection, TrailElement>.Accessor _headAccessor;

        public static void SetTrailHeadData(this TrailElementCollection trailElementCollection, Vector3 start, Vector3 end, float time)
        {
            if (UnityGame.GameVersion >= NewTrailApiVersion)
                trailElementCollection.SetHeadData(start, end, time);
            else
            {
                if (_headAccessor is null)
                    _headAccessor = FieldAccessor<TrailElementCollection, TrailElement>.GetAccessor("head");

                _headAccessor(ref trailElementCollection).SetData(start, end, time);
            }
        }
    }

    internal class ClawTrail : SaberTrail
    {
        // The prefab isn't available, so SaberTrail.Awake would fail
        public override void Awake() { }

        SaberTrailRenderer _leftClawRenderer, _rightClawRenderer;

        const float TrailYOffset = 0.0075f;
        const float TrailXOffset = 0.025f;
        static float TrailLength => Preferences.Length * 0.75f;

        public void RegisterPrefab(SaberTrailRenderer prefab)
        {
            _trailRendererPrefab = prefab;
        }
        public override void Init()
        {
            if (!_trailRenderer)
                _trailRenderer = Instantiate(_trailRendererPrefab, new Vector3(0, TrailYOffset), Quaternion.identity);
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
            _trailRenderer.Init(trailWidth, _trailDuration, _granularity, _whiteSectionMaxDuration);
            _leftClawRenderer.Init(trailWidth, _trailDuration * 0.65f, _granularity, _whiteSectionMaxDuration * 0.65f);
            _rightClawRenderer.Init(trailWidth, _trailDuration * 0.65f, _granularity, _whiteSectionMaxDuration * 0.65f);
            _inited = true;
        }

        public override void LateUpdate()
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
                        _trailRenderer.SetTrailWidth(GetTrailWidth(lastAddedData));
                    }
                }
                var numSamples = (int)Mathf.Floor((lastAddedData.time - _lastTrailElementTime) / _sampleStep);
                for (var index = 0; index < numSamples; ++index)
                {
                    _lastTrailElementTime += _sampleStep;
                    var t = (lastAddedData.time - _lastTrailElementTime) / (lastAddedData.time - prevAddedData.time);

                    var bottom = Vector3.LerpUnclamped(lastAddedData.bottomPos, prevAddedData.bottomPos, t);
                    var top = Vector3.LerpUnclamped(lastAddedData.topPos, prevAddedData.topPos, t);

                    _trailElementCollection.SetTrailHeadData(bottom, CalcNewTopPos(bottom, top), _lastTrailElementTime);
                    _trailElementCollection.MoveTailToHead();
                }
                _trailElementCollection.SetTrailHeadData(lastAddedData.bottomPos, lastAddedData.topPos, lastAddedData.time);
                _trailElementCollection.UpdateDistances();

                _trailRenderer.transform.position = transform.rotation * new Vector3(0, TrailYOffset);
                _leftClawRenderer.transform.position = transform.rotation * new Vector3(-TrailXOffset, TrailYOffset);
                _rightClawRenderer.transform.position = transform.rotation * new Vector3(TrailXOffset, TrailYOffset);

                _trailRenderer.UpdateMesh(_trailElementCollection, _color);
                _leftClawRenderer.UpdateMesh(_trailElementCollection, _color);
                _rightClawRenderer.UpdateMesh(_trailElementCollection, _color);
            }
        }

        Vector3 CalcNewTopPos(Vector3 bottom, Vector3 top)
        {
            return bottom + (top - bottom).normalized * TrailLength;
        }

        public override void OnEnable()
        {
            if (_inited)
            {
                ResetTrailData();
                _trailRenderer.UpdateMesh(_trailElementCollection, _color);
                _leftClawRenderer.UpdateMesh(_trailElementCollection, _color);
                _rightClawRenderer.UpdateMesh(_trailElementCollection, _color);
            }
            if (_trailRenderer)
                _trailRenderer.enabled = true;
            if (_leftClawRenderer)
                _leftClawRenderer.enabled = true;
            if (_rightClawRenderer)
                _rightClawRenderer.enabled = true;
        }

        public override void OnDisable()
        {
            if (_trailRenderer)
                _trailRenderer.enabled = false;
            if (_leftClawRenderer)
                _leftClawRenderer.enabled = false;
            if (_rightClawRenderer)
                _rightClawRenderer.enabled = false;
        }

        public override void OnDestroy()
        {
            if (_trailRenderer)
                Destroy(_trailRenderer.gameObject);
            if (_leftClawRenderer)
                Destroy(_leftClawRenderer.gameObject);
            if (_rightClawRenderer)
                Destroy(_rightClawRenderer.gameObject);
        }

        public override void ResetTrailData()
        {
            var lastAddedData = _movementData.lastAddedData;
            var bottomPos = lastAddedData.bottomPos;
            var topPos = lastAddedData.topPos;
            _lastTrailElementTime = lastAddedData.time;
            _trailElementCollection.InitSnapshots(bottomPos, CalcNewTopPos(bottomPos, topPos), _lastTrailElementTime);
        }

        public override float GetTrailWidth(BladeMovementDataElement lastAddedData) => TrailLength;
    }
}

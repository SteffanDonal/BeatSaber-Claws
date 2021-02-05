using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Claws.Modifiers
{
    class ClawTrail : SaberTrail
    {
        // The prefab isn't available, so SaberTrail.Awake would fail
        public override void Awake() { }

        private SaberTrailRenderer leftClawRenderer, rightClawRenderer;

        private static float TRAIL_Y_OFFSET = 0.0075f;
        private static float TRAIL_X_OFFSET = 0.025f;
        private static float TRAIL_LENGTH => Preferences.Length * 0.75f;

        public void RegisterPrefab(SaberTrailRenderer prefab)
        {
            _trailRendererPrefab = prefab;
        }
        public override void Init()
        {
            if (!_trailRenderer)
                _trailRenderer = UnityEngine.Object.Instantiate<SaberTrailRenderer>(_trailRendererPrefab, new Vector3(0, TRAIL_Y_OFFSET), Quaternion.identity);
            if (!leftClawRenderer)
                leftClawRenderer = UnityEngine.Object.Instantiate<SaberTrailRenderer>(_trailRendererPrefab, new Vector3(-TRAIL_X_OFFSET, TRAIL_Y_OFFSET), Quaternion.identity);
            if (!rightClawRenderer)
                rightClawRenderer = UnityEngine.Object.Instantiate<SaberTrailRenderer>(_trailRendererPrefab, new Vector3(TRAIL_X_OFFSET, TRAIL_Y_OFFSET), Quaternion.identity);

            _sampleStep = 1f / _samplingFrequency;
            BladeMovementDataElement lastAddedData = _movementData.lastAddedData;
            Vector3 bottomPos = lastAddedData.bottomPos;
            Vector3 topPos = lastAddedData.topPos;
            _lastTrailElementTime = lastAddedData.time;
            _trailElementCollection = new TrailElementCollection(Mathf.CeilToInt(_samplingFrequency * _trailDuration) + 3, bottomPos, calcNewTopPos(bottomPos, topPos), _lastTrailElementTime);
            float trailWidth = GetTrailWidth(lastAddedData);
            _whiteSectionMaxDuration = Math.Min(_whiteSectionMaxDuration, _trailDuration);
            _lastZScale = transform.lossyScale.z;
            _trailRenderer.Init(trailWidth, _trailDuration, _granularity, _whiteSectionMaxDuration);
            leftClawRenderer.Init(trailWidth, _trailDuration, _granularity, _whiteSectionMaxDuration);
            rightClawRenderer.Init(trailWidth, _trailDuration, _granularity, _whiteSectionMaxDuration);
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
                BladeMovementDataElement lastAddedData = _movementData.lastAddedData;
                BladeMovementDataElement prevAddedData = _movementData.prevAddedData;
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
                int numSamples = (int)Mathf.Floor((lastAddedData.time - _lastTrailElementTime) / _sampleStep);
                for (int index = 0; index < numSamples; ++index)
                {
                    _lastTrailElementTime += _sampleStep;
                    float t = (lastAddedData.time - _lastTrailElementTime) / (lastAddedData.time - prevAddedData.time);

                    Vector3 bottom = Vector3.LerpUnclamped(lastAddedData.bottomPos, prevAddedData.bottomPos, t);
                    Vector3 top = Vector3.LerpUnclamped(lastAddedData.topPos, prevAddedData.topPos, t);

                    _trailElementCollection.head.SetData(bottom, calcNewTopPos(bottom, top), _lastTrailElementTime);
                    _trailElementCollection.MoveTailToHead();
                }
                _trailElementCollection.head.SetData(lastAddedData.bottomPos, lastAddedData.topPos, lastAddedData.time);
                _trailElementCollection.UpdateDistances();

                _trailRenderer.transform.position = this.transform.rotation * new Vector3(0, TRAIL_Y_OFFSET);
                leftClawRenderer.transform.position = this.transform.rotation * new Vector3(-TRAIL_X_OFFSET, TRAIL_Y_OFFSET);
                rightClawRenderer.transform.position = this.transform.rotation * new Vector3(TRAIL_X_OFFSET, TRAIL_Y_OFFSET);

                _trailRenderer.UpdateMesh(_trailElementCollection, _color);
                leftClawRenderer.UpdateMesh(_trailElementCollection, _color);
                rightClawRenderer.UpdateMesh(_trailElementCollection, _color);
            }
        }

        private Vector3 calcNewTopPos(Vector3 bottom, Vector3 top)
        {
            return bottom + (top - bottom).normalized * TRAIL_LENGTH;
        }

        public override void OnEnable()
        {
            if (_inited)
            {
                ResetTrailData();
                _trailRenderer.UpdateMesh(_trailElementCollection, _color);
                leftClawRenderer.UpdateMesh(_trailElementCollection, _color);
                rightClawRenderer.UpdateMesh(_trailElementCollection, _color);
            }
            if (_trailRenderer)
                _trailRenderer.enabled = true;
            if (leftClawRenderer)
                leftClawRenderer.enabled = true;
            if (rightClawRenderer)
                rightClawRenderer.enabled = true;
        }

        public override void OnDisable()
        {
            if (_trailRenderer)
                _trailRenderer.enabled = false;
            if (leftClawRenderer)
                leftClawRenderer.enabled = false;
            if (rightClawRenderer)
                rightClawRenderer.enabled = false;
        }

        public override void OnDestroy()
        {
            if (_trailRenderer)
                UnityEngine.Object.Destroy(_trailRenderer.gameObject);
            if (leftClawRenderer)
                UnityEngine.Object.Destroy(leftClawRenderer.gameObject);
            if (rightClawRenderer)
                UnityEngine.Object.Destroy(rightClawRenderer.gameObject);
        }

        public override void ResetTrailData()
        {
            BladeMovementDataElement lastAddedData = _movementData.lastAddedData;
            Vector3 bottomPos = lastAddedData.bottomPos;
            Vector3 topPos = lastAddedData.topPos;
            _lastTrailElementTime = lastAddedData.time;
            _trailElementCollection.InitSnapshots(bottomPos, calcNewTopPos(bottomPos, topPos), _lastTrailElementTime);
        }

        public override float GetTrailWidth(BladeMovementDataElement lastAddedData) => TRAIL_LENGTH;
    }
}

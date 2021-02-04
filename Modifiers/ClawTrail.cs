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

        private static readonly Vector3 CLAW_OFFSET = new Vector3(0.025f, 0f);

        public void RegisterPrefab(SaberTrailRenderer prefab)
        {
            _trailRendererPrefab = prefab;
        }
        public override void Init()
        {
            if (!_trailRenderer)
                _trailRenderer = UnityEngine.Object.Instantiate<SaberTrailRenderer>(_trailRendererPrefab, Vector3.zero, Quaternion.identity);
            if (!leftClawRenderer)
                leftClawRenderer = UnityEngine.Object.Instantiate<SaberTrailRenderer>(_trailRendererPrefab, -CLAW_OFFSET, Quaternion.identity);
            if (!rightClawRenderer)
                rightClawRenderer = UnityEngine.Object.Instantiate<SaberTrailRenderer>(_trailRendererPrefab, CLAW_OFFSET, Quaternion.identity);

            _sampleStep = 1f / _samplingFrequency;
            BladeMovementDataElement lastAddedData = _movementData.lastAddedData;
            Vector3 bottomPos = lastAddedData.bottomPos;
            Vector3 topPos = lastAddedData.topPos;
            _lastTrailElementTime = lastAddedData.time;
            _trailElementCollection = new TrailElementCollection(Mathf.CeilToInt(_samplingFrequency * _trailDuration) + 3, bottomPos, topPos, _lastTrailElementTime);
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

                    _trailElementCollection.head.SetData(Vector3.LerpUnclamped(lastAddedData.bottomPos, prevAddedData.bottomPos, t), Vector3.LerpUnclamped(lastAddedData.topPos, prevAddedData.topPos, t), _lastTrailElementTime);
                    _trailElementCollection.MoveTailToHead();
                }
                _trailElementCollection.head.SetData(lastAddedData.bottomPos, lastAddedData.topPos, lastAddedData.time);
                _trailElementCollection.UpdateDistances();

                leftClawRenderer.transform.position = this.transform.rotation * -CLAW_OFFSET;
                rightClawRenderer.transform.position = this.transform.rotation * CLAW_OFFSET;

                _trailRenderer.UpdateMesh(_trailElementCollection, _color);
                leftClawRenderer.UpdateMesh(_trailElementCollection, _color);
                rightClawRenderer.UpdateMesh(_trailElementCollection, _color);
            }
        }
    }
}

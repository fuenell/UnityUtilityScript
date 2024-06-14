using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SmoothSlider : MonoBehaviour
{
    public float _smoothingSpeed = 5.0f; // 부드러운 이동 속도 조절

    [SerializeField]
    private Slider _targetSlider;
    private Slider _slider;
    private bool _isSliderMoved;
    private float _targetValue;

    private void Awake()
    {
        this._slider = GetComponent<Slider>();
        _slider.minValue = _targetSlider.minValue;
        _slider.maxValue = _targetSlider.maxValue;
        //_slider.value = (_slider.minValue + _slider.maxValue) * 0.5f;
    }

    private void OnEnable()
    {
        _isSliderMoved = false;
        _slider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    private void OnDisable()
    {
        _slider.onValueChanged.RemoveListener(OnSliderValueChanged);
    }

    private void Update()
    {
        if (_isSliderMoved)
        {
            _targetSlider.value = Mathf.Lerp(_targetSlider.value, _targetValue, _smoothingSpeed * Time.deltaTime);
        }
    }

    // 슬라이더를 조작할 때 호출되는 함수
    public void OnSliderValueChanged(float newValue)
    {
        StopAllCoroutines();

        // 타겟 값을 슬라이더의 현재 값으로 설정
        _targetValue = newValue;
        _isSliderMoved = true;

        StartCoroutine(DelayCorrect(1));
    }

    private IEnumerator DelayCorrect(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        _isSliderMoved = false;

        float startValue = _targetSlider.value;
        float moveTime = Mathf.Abs(startValue - _targetValue) / _smoothingSpeed * 0.01f;
        for (float time = 0; time <= moveTime;)
        {
            time += Time.deltaTime;
            _targetSlider.value = Mathf.Lerp(startValue, _targetValue, time / moveTime); ;
            yield return null;
        }
    }
}

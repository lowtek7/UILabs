using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using System.Linq;

public class UITransitionEffect : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private int gridWidth = 10;
    [SerializeField] private int gridHeight = 8;
    [SerializeField] private float tileSize = 64f;
    [SerializeField] private float transitionDuration = 1f;
    [SerializeField] private float delayBetweenTiles = 0.015f;

    private List<GameObject> tiles = new List<GameObject>();
    private RectTransform canvasRect;
    private CancellationTokenSource _cts = new CancellationTokenSource();

    void Awake()
    {
        canvasRect = GetComponent<RectTransform>();
        CreateTileGridAsync().Forget(ex => Debug.LogException(ex));
    }

    private void OnDestroy()
    {
        _cts.Cancel();
        _cts.Dispose();
    }

    private async UniTask CreateTileGridAsync()
    {
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                GameObject tile = Instantiate(tilePrefab, transform);
                RectTransform tileRect = tile.GetComponent<RectTransform>();

                float isoX = (x - y) * tileSize * 0.5f;
                float isoY = (x + y) * tileSize * 0.25f;

                tileRect.anchoredPosition = new Vector2(isoX, isoY);

                Image tileImage = tile.GetComponent<Image>();
                tileImage.color = new Color(0, 0, 0, 0); // 완전 투명으로 시작

                tiles.Add(tile);

                if ((y * gridWidth + x) % 20 == 0)
                {
                    await UniTask.NextFrame(_cts.Token);
                }
            }
        }
    }

    public async UniTask TransitionToNewUIAsync(CanvasGroup currentUI, CanvasGroup targetUI)
    {
        try
        {
            // 현재 UI는 그대로 두고 다음 UI는 숨김
            currentUI.gameObject.SetActive(true);
            targetUI.gameObject.SetActive(false);

            // 타일 초기화
            foreach (var tile in tiles)
            {
                var tileImage = tile.GetComponent<Image>();
                tileImage.color = new Color(0, 0, 0, 0);
            }

            // 타일을 오른쪽에서 왼쪽으로 정렬
            var sortedTiles = tiles.OrderByDescending(t =>
                t.GetComponent<RectTransform>().anchoredPosition.x
            ).ToList();

            // 다음 UI 활성화
            targetUI.gameObject.SetActive(true);

            // 타일 페이드인
            foreach (var tile in sortedTiles)
            {
                FadeTileAsync(tile, true).Forget();
                await UniTask.Delay(System.TimeSpan.FromSeconds(delayBetweenTiles), cancellationToken: _cts.Token);
            }

            // 전환이 완료될 때까지 대기
            await UniTask.Delay(System.TimeSpan.FromSeconds(transitionDuration + 0.1f), cancellationToken: _cts.Token);

            // 타일 페이드아웃
            foreach (var tile in tiles)
            {
                FadeTileAsync(tile, false).Forget();
            }

            // 이전 UI 비활성화
            currentUI.gameObject.SetActive(false);

            // 페이드아웃 완료 대기
            await UniTask.Delay(System.TimeSpan.FromSeconds(0.5f), cancellationToken: _cts.Token);
        }
        catch (System.OperationCanceledException)
        {
            Debug.Log("Transition cancelled");
        }
    }

    private async UniTask FadeTileAsync(GameObject tile, bool fadeIn)
    {
        var image = tile.GetComponent<Image>();
        float startAlpha = fadeIn ? 0f : 1f;
        float targetAlpha = fadeIn ? 1f : 0f;
        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = elapsedTime / transitionDuration;
            float currentAlpha = Mathf.Lerp(startAlpha, targetAlpha, normalizedTime);

            image.color = new Color(0, 0, 0, currentAlpha);

            await UniTask.NextFrame(_cts.Token);
        }

        image.color = new Color(0, 0, 0, targetAlpha);
    }
}

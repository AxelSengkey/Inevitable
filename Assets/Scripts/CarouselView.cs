using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CarouselView : MonoBehaviour
{
    public RectTransform[] images;
    public RectTransform viewWindow;

    private bool canSwipe;
    private float imageWidth;
    private float lerpTimer, lerpPosition;
    private float mousePositionStartX, mousePositionEndX, dragAmount;
    private float screenPosition, lastScreenPosition;

    public float image_gap = 30;

    public int swipeThrustHold = 30;

    private int currentIndex, currentIndex1;

    public static int curLevelIndex;

    // Just for display purpose
    public TextMeshProUGUI currentIndexLable;

    // Start is called before the first frame update
    void Start()
    {
        imageWidth = viewWindow.rect.width;
        for (int i = 1; i < images.Length; i++)
        {
            images[i].anchoredPosition = new Vector2(((imageWidth + image_gap) * i), 0);
        }
    }

    // Update is called once per frame
    void Update()
    {

        UpdateCurrentIndexLable();
        UpdateCarouselView();
        curLevelIndex = currentIndex1;
    }

    void UpdateCurrentIndexLable()
    {
        if (currentIndexLable)
            currentIndex1 = currentIndex + 1;
        currentIndexLable.text = currentIndex1.ToString() + " / " + images.Length.ToString();
    }

    void UpdateCarouselView()
    {
        lerpTimer = lerpTimer + Time.unscaledDeltaTime;

        if (lerpTimer < 0.333f)
        {
            screenPosition = Mathf.Lerp(lastScreenPosition, lerpPosition * -1, lerpTimer * 3);
            lastScreenPosition = screenPosition;
        }

        if (Input.GetMouseButtonDown(0) || (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow)))
        {
            canSwipe = true;
            mousePositionStartX = Input.mousePosition.x;
        }


        if (Input.GetMouseButton(0) || (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow)))
        {
            if (canSwipe)
            {
                if (Input.GetMouseButton(0))
                {
                    mousePositionEndX = Input.mousePosition.x;
                    dragAmount = mousePositionEndX - mousePositionStartX;
                }
                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    dragAmount = swipeThrustHold + 1;
                }
                else if (Input.GetKey(KeyCode.RightArrow))
                {
                    dragAmount = -(swipeThrustHold + 1);
                }
                screenPosition = lastScreenPosition + dragAmount;
            }
        }

        if (Mathf.Abs(dragAmount) > swipeThrustHold && canSwipe)
        {
            canSwipe = false;
            lastScreenPosition = screenPosition;
            if (currentIndex < images.Length)
                OnSwipeComplete();
            else if (currentIndex == images.Length && dragAmount < 0)
                lerpTimer = 0;
            else if (currentIndex == images.Length && dragAmount > 0)
                OnSwipeComplete();
        }

        for (int i = 0; i < images.Length; i++)
        {
            images[i].anchoredPosition = new Vector2(screenPosition + ((imageWidth + image_gap) * i), 0);
            if (i == currentIndex)
            {
                images[i].localScale = Vector3.Lerp(images[i].localScale, viewWindow.localScale, Time.unscaledDeltaTime * 5);
            }
            else
            {
                images[i].localScale = Vector3.Lerp(images[i].localScale, new Vector3(0.7f, 0.7f, 0.7f), Time.unscaledDeltaTime * 5);
            }
        }
    }

    void OnSwipeComplete()
    {
        lastScreenPosition = screenPosition;

        if (dragAmount > 0)
        {
            if (dragAmount >= swipeThrustHold)
            {
                if (currentIndex == 0)
                {
                    lerpTimer = 0; lerpPosition = 0;
                }
                else
                {
                    currentIndex--;
                    lerpTimer = 0;
                    if (currentIndex < 0)
                        currentIndex = 0;
                    lerpPosition = (imageWidth + image_gap) * currentIndex;
                }
            }
            else
            {
                lerpTimer = 0;
            }
        }
        else if (dragAmount < 0)
        {
            if (Mathf.Abs(dragAmount) >= swipeThrustHold)
            {
                if (currentIndex == images.Length - 1)
                {
                    lerpTimer = 0;
                    lerpPosition = (imageWidth + image_gap) * currentIndex;
                }
                else
                {
                    lerpTimer = 0;
                    currentIndex++;
                    lerpPosition = (imageWidth + image_gap) * currentIndex;
                }
            }
            else
            {
                lerpTimer = 0;
            }
        }
        dragAmount = 0;
    }
}

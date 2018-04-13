using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapSizeController : MonoBehaviour
{
    public InputField widthInput;
    public InputField heightInput;

    private int width,
                height;

    public void ValidateWidthChanged()
    {
        if (!int.TryParse(widthInput.text, out width))
        {
            widthInput.text = "";
        }
    }

    public void ValidateWidthEndEdit()
    {
        if (int.TryParse(widthInput.text, out width) && (width > 30 || width < 13))
        {
            widthInput.text = "15";
            width = 15;
        }
    }

    public void ValidateHeightChanged()
    {
        if (!int.TryParse(heightInput.text, out height))
        {
            heightInput.text = "";
        }
    }

    public void ValidateHeightEndEdit()
    {
        if (int.TryParse(heightInput.text, out height) && (height > 30 || height < 13))
        {
            heightInput.text = "15";
            height = 15;
        }
    }
}

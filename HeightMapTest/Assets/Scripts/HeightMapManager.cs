using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightMapManager : MonoBehaviour {

	private float[] data;
	private ushort[] rawData;
	private Color[] colorData;
	private bool pendingData;

	public string imageName;
	private Texture2D texture;

	// mesh kann nicht mehr als 65000 vertices haben
    private const int downsampleSize = 8;
    private const float depthScale = 0.1f; // 0.02 für Bilder

	private const int ORIGINAL_WIDTH = 1920;
	private const int ORIGINAL_HEIGHT = 1080;
	public const int WIDTH = ORIGINAL_WIDTH / downsampleSize;
	public const int HEIGHT = ORIGINAL_HEIGHT / downsampleSize;

	// timer
    private float timer;
    public float updateInterval = 1;

	void Start () {
		data = new float[WIDTH * HEIGHT];
		rawData = new ushort[ORIGINAL_WIDTH * ORIGINAL_HEIGHT];
		colorData = new Color[WIDTH * HEIGHT] ;

		texture = Resources.Load(imageName) as Texture2D;
	}
	
	void OnGUI() {
		//GUI.Box(new Rect(10,10,100,90), "Loader Menu");
		//GUI.DrawTexture(new Rect(0, 0, 500, 200), texture);
	}
	private bool firstTime = true;
	void Update () {
		if (timer > updateInterval || firstTime) {
			firstTime = false;
			pendingData = true;

			//randomData();
			rgbData();

			downsampleAndScaleData();

			timer = 0;
		}
        timer += Time.deltaTime;
	}

	private void randomData() {
		for (int y = 0; y < ORIGINAL_HEIGHT; y ++)
		{
			for (int x = 0; x < ORIGINAL_WIDTH; x ++)
			{
				// rawData[(y * _Width) + x] = (ushort) (x + y);
				rawData[(y * ORIGINAL_WIDTH) + x] = (ushort) (Random.value * 10);
				// rawData[(y * _Width) + x] = (ushort) ((10 * Mathf.Sin(Mathf.Pow(x, 2) + Mathf.Pow(y, 2)) / 10));
				// rawData[(y * _Width) + x] = (ushort) ((Mathf.Sin(Mathf.Deg2Rad * (5f * x)) * Mathf.Cos(Mathf.Deg2Rad * (5f * y)) / 5f) * 5f) ;
			}
		}
	}

	private void rgbData() {
		for (int y = 0; y < ORIGINAL_HEIGHT; y ++)
		{
			for (int x = 0; x < ORIGINAL_WIDTH; x ++)
			{
				Color pixel = texture.GetPixel(x, y);
				// int red = (int) (pixel.r * 255);
				// int green = (int) (pixel.g * 255);
				// int blue = (int) (pixel.b * 255);
				// int rgb = (red << 16) + (green << 8) + blue;

				// rawData[(y * ORIGINAL_WIDTH) + x] = (ushort) (rgb / 3894); // geteilt durch 3894, damit Wert zwischen 0 und 65535 liegt
				rawData[(y * ORIGINAL_WIDTH) + x] = (ushort) (pixel.r * 1000 + pixel.g * 100 + pixel.b * 10);

				if (x % downsampleSize == 0 && y % downsampleSize == 0) {
					colorData[((y / downsampleSize) * WIDTH) + (x / downsampleSize)] = pixel;
				}
			}
		}
	}

	private void downsampleAndScaleData() {
		for (int y = 0; y < ORIGINAL_HEIGHT; y += downsampleSize)
        {
            for (int x = 0; x < ORIGINAL_WIDTH; x += downsampleSize)
            {
				int indexX = x / downsampleSize;
				int indexY = y / downsampleSize;
				int smallIndex = (indexY * (ORIGINAL_WIDTH / downsampleSize)) + indexX;
				
				float avg = GetAvg(rawData, x, y, ORIGINAL_WIDTH, ORIGINAL_HEIGHT);
				// float avg = rawData[(y * ORIGINAL_WIDTH) + x];
				
				avg = avg * depthScale;

				if (avg == 0) avg = 1;

				data[smallIndex] = avg;
			}
		}
	}

	private float GetAvg(ushort[] rawData, int x, int y, int width, int height)
    {
        float sum = 0f;
        
        for (int y1 = y; y1 < y + downsampleSize; y1++)
        {
            for (int x1 = x; x1 < x + downsampleSize; x1++)
            {
                int fullIndex = (y1 * width) + x1;
                sum += rawData[fullIndex];
            }
        }

        return sum / (downsampleSize * downsampleSize);
    }

	public bool HasPendingData() {
		return pendingData;
	}

	public float[] GetData() {
		pendingData = false;
		return data;
	}

	public Color[] GetColorData() {
		return colorData;
	}

}

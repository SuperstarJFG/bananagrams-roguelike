using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;

public class TileManager : MonoBehaviour
{
    // Parent object for all tiles
    [SerializeField] GameObject tileParent;

    [SerializeField] GameObject tilePrefab;
    static GameObject[] letterPrefabs = new GameObject[26];

    static GameObject selectedTile;

    public List<GameObject> placedTiles = new();

    HashSet<GameObject> connectedTiles = new();
    [SerializeField] GameObject ConnectionAlert;

    static Vector3 selectedTileStartPosition;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // init letter prefabs
        for (int i = 0; i < 26; i++)
        {
            letterPrefabs[i] = Instantiate(tilePrefab);
            letterPrefabs[i].SetActive(false);
            letterPrefabs[i].GetComponent<LetterController>().letter = (char)(i + 'A');
            letterPrefabs[i].GetComponentInChildren<TMP_Text>().text = ((char)(i + 'A')).ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Generate random tile
        if (Input.GetKeyDown(KeyCode.R))
        {
            char letter = WordChecker.RandomLetter();
            var tile = Instantiate(letterPrefabs[letter - 'A']);
            tile.transform.position = GetMouseGridPosition();
            tile.SetActive(true);
            placedTiles.Add(tile);

            GetComponent<WordReader>().ReadWords();
        }

        // Selected tile follows cursor
        if (selectedTile != null)
        {
            selectedTile.transform.position = GetMouseGridPosition();
            SelectedTileEffects();
        }

        // Pick up tile
        if (Input.GetMouseButtonDown(0) && GetTile() != null)
        {
            selectedTileStartPosition = GetTile().transform.position;
            selectedTile = GetTile();
            placedTiles.Remove(selectedTile);
         
            GetComponent<WordReader>().ReadWords();
        }

        // Put down tile
        if (Input.GetMouseButtonUp(0) && selectedTile != null)
        {
            PlaceTile();
         
            GetComponent<WordReader>().ReadWords();
        }

        // Create tile
        if (Input.GetMouseButton(1))
        {
            if (selectedTile == null && GetTile() == null)
            {
                selectedTile = Instantiate(letterPrefabs[Random.Range(0,26)], tileParent.transform);
                selectedTile.transform.position = GetMouseGridPosition();
                selectedTile.SetActive(true);
                PlaceTile();
            }

            GetComponent<WordReader>().ReadWords();
        }

        // Erase tile
        if (Input.GetKey(KeyCode.E))
        {
            DeleteTile();

            GetComponent<WordReader>().ReadWords();
        }

        // Check if all tiles are connected
        if (AreAllTilesConnected() || placedTiles.Count == 0 || WordReader.IncorrectCount > 0)
        {
            ConnectionAlert.SetActive(false);
        }
        else
        {
            ConnectionAlert.SetActive(true);
        }
    }

    static Vector3 GetMouseGridPosition()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Dont allow mousePosition to be offscreen
        mousePosition.x = Mathf.Clamp(mousePosition.x, -9f, 9f);
        mousePosition.y = Mathf.Clamp(mousePosition.y, -4f, 4f);

        return new Vector3(Mathf.Round(mousePosition.x), Mathf.Round(mousePosition.y), 0f);
    }

    void PlaceTile()
    {
        if (GetTile() != null)
        {
            selectedTile.transform.position = selectedTileStartPosition;
        }

        SpriteRenderer spriteRenderer = selectedTile.GetComponent<SpriteRenderer>();
        Color selectedTileColor = spriteRenderer.color;
        selectedTileColor.a = 1f;
        spriteRenderer.color = selectedTileColor;
        spriteRenderer.sortingOrder = 0;

        Vector3 position = selectedTile.transform.position;
        position.z = 0;
        selectedTile.transform.position = position;

        placedTiles.Add(selectedTile);
        
        selectedTile = null;
    }

    void DeleteTile()
    {
        foreach (GameObject tile in placedTiles)
        {
            if (tile.transform.position == GetMouseGridPosition())
            {
                placedTiles.Remove(tile);
                Destroy(tile);
                return;
            }
        }
    }

    // Get the tile that the mouse is hovering over
    GameObject GetTile()
    {
        foreach (GameObject tile in placedTiles)
        {
            if (tile.transform.position == GetMouseGridPosition())
            {
                return tile;
            }
        }
        return null;
    }

    bool AreAllTilesConnected()
    {
        if (placedTiles.Count == 0)
        {
            return false;
        }
        connectedTiles.Clear();
        SetConnectedTiles(placedTiles[0]);
        return connectedTiles.SetEquals(placedTiles);
    }

    void SetConnectedTiles(GameObject parentTile)
    {
        connectedTiles.Add(parentTile);
        foreach (GameObject childTile in placedTiles)
        {
            if (Vector3.Distance(parentTile.transform.position, childTile.transform.position) <= 1f)
            {
                if (!connectedTiles.Contains(childTile))
                {
                    SetConnectedTiles(childTile);
                }
            }
        }
    }

    // Make selected tile transparent and on top
    void SelectedTileEffects()
    {
        Vector3 position = selectedTile.transform.position;
        position.z = -5f;
        selectedTile.transform.position = position;

        // Add selected tile's sprites to list
        List<SpriteRenderer> selectedTileSpriteRenderers = new();
        List<SpriteShapeRenderer> selectedTileSpriteShapeRenderers = new();
        foreach (SpriteRenderer spriteRenderer in selectedTile.GetComponents<SpriteRenderer>())
        {
            selectedTileSpriteRenderers.Add(spriteRenderer);
        }
        foreach (SpriteShapeRenderer spriteShapeRenderer in selectedTile.GetComponents<SpriteShapeRenderer>())
        {
            selectedTileSpriteShapeRenderers.Add(spriteShapeRenderer);
        }
        foreach (SpriteRenderer spriteRenderer in selectedTile.GetComponentsInChildren<SpriteRenderer>())
        {
            selectedTileSpriteRenderers.Add(spriteRenderer);
        }
        foreach (SpriteShapeRenderer spriteShapeRenderer in selectedTile.GetComponentsInChildren<SpriteShapeRenderer>())
        {
            selectedTileSpriteShapeRenderers.Add(spriteShapeRenderer);
        }

        // Make all sprites in list transparent and on top
        foreach (SpriteRenderer spriteRenderer in selectedTileSpriteRenderers)
        {
            Color selectedTileColor = spriteRenderer.color;
            selectedTileColor.a = 0.2f;
            spriteRenderer.color = selectedTileColor;
            //spriteRenderer.sortingOrder = 1;
        }
        foreach (SpriteShapeRenderer spriteShapeRenderer in selectedTileSpriteShapeRenderers)
        {
            Color selectedTileColor = spriteShapeRenderer.color;
            selectedTileColor.a = 0.2f;
            spriteShapeRenderer.color = selectedTileColor;
            //spriteShapeRenderer.sortingOrder = 1;
        }
    }
}

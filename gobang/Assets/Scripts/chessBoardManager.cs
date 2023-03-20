using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class chessBoardManager : MonoBehaviour
{

    public bool isSinglePlayer = true;
    bool isWhite = true;
    bool moveable = true;
    Vector2 offset = new Vector2(-3.969f, -3.969f);
    float cell_size = 0.441f;

    public GameObject white_pawn;
    public GameObject black_pawn;
    public GameObject canvas_gameover;
    public GameObject canvas_ui;
    public AudioSource audioSource;
    public AudioClip audioClip;
    int[,] chessBoard_Matrix = new int[19, 19];         // white = 1, black = -1
    Transform[,] transform_Matrix = new Transform[19, 19];


    IEnumerator ai_turn(float wait_time)
    {

        yield return new WaitForSeconds(wait_time);
        ai_random_move();
        moveable = true;

    }


    void play_sound()
    {
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    void gameover() {
        canvas_gameover.SetActive(true);
        canvas_ui.SetActive(false);

    }


    void check_win_condition(Vector2 vec2) {
        int counter;
        int pos_x = Mathf.RoundToInt(vec2.x);
        int pos_y = Mathf.RoundToInt(vec2.y);
        int x, y;
        int pawn;
        if (isWhite)
            pawn = 1;
        else
            pawn = -1;

        // check hori
        x = pos_x;
        y = pos_y;
        counter = 0;
        while (0 <= x && x <= 18 && chessBoard_Matrix[x, y] == pawn)
        {
            x--;
        }
        x++;
        while (0 <= x && x <= 18 && chessBoard_Matrix[x, y] == pawn)
        {
            x++;
            counter++;
        }
        if (counter >= 5) {
            gameover();
        }


        // check vert
        x = pos_x;
        y = pos_y;
        counter = 0;
        while (0 <= y && y <= 18 && chessBoard_Matrix[x, y] == pawn)
        {
            y--;
        }
        y++;
        while (0 <= y && y <= 18 && chessBoard_Matrix[x, y] == pawn)
        {
            y++;
            counter++;
        }
        if (counter >= 5)
        {
            gameover();
        }

        // check ¥´±× LB-RT
        x = pos_x;
        y = pos_y;
        counter = 0;
        while (0 <= x && x <= 18 && 0 <= y && y <= 18 && chessBoard_Matrix[x, y] == pawn)
        {
            x--;
            y--;
        }
        x++;
        y++;
        while (0 <= x && x <= 18 && 0 <= y && y <= 18 && chessBoard_Matrix[x, y] == pawn)
        {
            x++;
            y++;
            counter++;
        }
        if (counter >= 5)
        {
            gameover();
        }

        // check ¥´±× LT-RB
        x = pos_x;
        y = pos_y;
        counter = 0;
        while (0 <= x && x <= 18 && 0 <= y && y <= 18 && chessBoard_Matrix[x, y] == pawn)
        {
            x--;
            y++;
        }
        x++;
        y--;
        while (0 <= x && x <= 18 && 0 <= y && y <= 18 && chessBoard_Matrix[x, y] == pawn)
        {
            x++;
            y--;
            counter++;
        }
        if (counter >= 5)
        {
            gameover();
        }
    }

    void ai_random_move() {
        while(true)
        {
            int random_x = Random.Range(0, 19);
            int random_y = Random.Range(0, 19);
            Vector2 vec2 = new Vector2(random_x, random_y);
            if (chessBoard_Matrix[random_x, random_y] == 0) {
                chessBoard_Matrix[random_x, random_y] = -1;
                transform_Matrix[random_x, random_y].gameObject.SetActive(false);
                Instantiate(black_pawn, index_pos(vec2), Quaternion.identity);
                play_sound();
                check_win_condition(vec2);
                break;
            }

        }

    }

    public void make_a_move(Vector3 vec3) {
        Vector2 vec2 = pos_index(vec3);
        int x = Mathf.RoundToInt(vec2.x);
        int y = Mathf.RoundToInt(vec2.y);
        if (isWhite){
            Instantiate(white_pawn, vec3, Quaternion.identity);
            play_sound();
            chessBoard_Matrix[x, y] = 1;
            transform_Matrix[x, y].gameObject.SetActive(false);
        }
        else {
            Instantiate(black_pawn, vec3, Quaternion.identity);
            play_sound();
            chessBoard_Matrix[x, y] = -1;
            transform_Matrix[x, y].gameObject.SetActive(false);
        }
        check_win_condition(vec2);

        if (isWhite){
            isWhite = false;
            if (isSinglePlayer)
            {
                StartCoroutine(ai_turn(0.5f));
                isWhite = true;
            }
            else {
                moveable = true;
            }

        }
        else{
            isWhite = true;
            moveable = true;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Transform child = transform.Find("Grids");
        if (child != null)
        {
            int x = 0, y = 0;
            Transform[] childTransforms = child.GetComponentsInChildren<Transform>();
            foreach (Transform childTransform in childTransforms)
            {
                if (!childTransform.gameObject.CompareTag("grid"))
                    continue;
                
                transform_Matrix[x, y] = childTransform;
                Vector2 childPosition = new Vector2(x * cell_size, y * cell_size);
                childPosition += offset;
                childTransform.position = childPosition;
                
                x++;
                if (x > 18)
                {
                    x = 0;
                    y++;
                }
            }
        }
    }

    Vector2 index_pos(Vector2 vec2) {
        Vector3 result = new Vector3(vec2.x * cell_size, vec2.y * cell_size, 0);
        result.x += offset.x;
        result.y += offset.y;

        return result;
    }

    Vector2 pos_index(Vector3 vec3) {

        Vector2 childPosition = vec3;
        Vector2 child_index = childPosition - offset;
        child_index.x /= cell_size;
        child_index.y /= cell_size;

        return child_index;

    }

    public void grid_clicked(Transform transform) {
        if (!moveable)
            return;

        moveable = false;
        make_a_move(transform.position);
    }


    // Update is called once per frame

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kretanje : MonoBehaviour
{
    int defaultKretanje = 5;
    public int brzinaKretanja;
    int defaultSkok = 6;
    public int jacinaSkoka;
    public GameObject Camera2D;
    public GameObject Camera3D;
    bool sideViewActive= true;
    bool delayActive = false;
    float delayTime = 0;
    int maxZivot = 100;
    int zivot;
    float maxOvisnost =50;
    float ovisnost;
    bool naDrogama = false;
    float trajanjeDroge = 0;

    private void Start()
    {
        zivot = maxZivot;
        ovisnost = 0;
        brzinaKretanja = defaultKretanje;
        jacinaSkoka = defaultSkok;
    }

    private void Update()
    {
        CameraView();
        EfektDroge();
    }

    void CameraView()
    {
        if (sideViewActive == true && delayActive == false) //Ako smo u modu SideViewa pokreni 2-D kontrole
        {
            Movement2D();
        }

        if (sideViewActive == false) //Ako smo u third person modu pokreni 3-D kontrole
        {
            if (delayActive == false)
            {
                Movement3D();
            }
        }
        if (delayActive == true) //Delay za promjenu kontrola
        {
            delayTime += Time.deltaTime;
            if (delayTime > 0.3)
            {
                delayActive = false;
            }
        }
    }

    void EfektDroge()
    {
        if(naDrogama==true)
        {
            trajanjeDroge += Time.deltaTime;
            if(trajanjeDroge < 30)
            {
                brzinaKretanja = 8;
                jacinaSkoka = 8;
            }
            if(trajanjeDroge >= 30)
            {
                brzinaKretanja = defaultKretanje;
                jacinaSkoka = defaultSkok;
                naDrogama = false;
            }
        }
    }

    void Movement2D() //2-D kontrole - zasad lijevo i desno
    {
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector3.right * brzinaKretanja * Time.deltaTime); //LIJEVO
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector3.left * brzinaKretanja * Time.deltaTime); //DESNO
        }
        if(Input.GetKeyDown(KeyCode.Space)) //TREBA NARIKTATI DA SE SAMO JEDNOM MOŽE SKOČITI
        {
            GetComponent<Rigidbody>().velocity += Vector3.up * jacinaSkoka; //SKOK dodaje rigidbodyiju velocity prema gore za 7 jedinica
        }
    }
    void Movement3D() // 3-D kontrole - zasad gore dolje lijevo i desno bez skoka
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector3.right * brzinaKretanja * Time.deltaTime); //UNAPRIJED
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector3.forward * brzinaKretanja * Time.deltaTime); //LIJEVO
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector3.back * brzinaKretanja * Time.deltaTime); //DESNO
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector3.left * brzinaKretanja * Time.deltaTime); // UNAZAD
        }
    }
    
    private void OnTriggerEnter(Collider other) // promjena perspektive pri prolazu kroz triger
    {
        if(other.gameObject.tag =="CameraSwitch3rd") //Promjena u pticju perspektivu
        {
            Camera2D.transform.gameObject.SetActive(false);
            Camera3D.transform.gameObject.SetActive(true);
            sideViewActive = false;
            delayActive = true;
        }
        if (other.gameObject.tag == "CameraSwitchSide") //Promjena u 2D
        {
            Camera3D.transform.gameObject.SetActive(false);
            Camera2D.transform.gameObject.SetActive(true);
            float x = gameObject.transform.position.x;
            float y = gameObject.transform.position.y;
            gameObject.transform.position = new Vector3(x, y, 0);
            sideViewActive = true;
            delayActive = true;
        }
        if(other.gameObject.tag == "Droga")
        {
            naDrogama = true;
            Destroy(other.gameObject);
        }
    }
}

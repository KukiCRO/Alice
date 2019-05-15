using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kretanje : MonoBehaviour
{
    //KRETANJE
    int defaultKretanje = 5;
    public int brzinaKretanja;
    int defaultSkok = 6;
    public int jacinaSkoka;
    bool delayActive = false;
    float delayTime = 0;
    bool isGrounded = true;
    bool uZraku = false;
    bool jumpDelayActive;
    float jumpDelay;


    //KAMERE
    public GameObject Camera2D;
    public GameObject Camera3D;
    bool sideViewActive = true;

    //ANIMATOR
    Animator anim_mac;

    //STATS
    int maxZivot = 100;
    int trenutniZivot;
    float maxOvisnost =50;
    float ovisnost;
    bool naDrogama = false;
    float trajanjeDroge = 0;
    float imunityDelay = 0;

    //SKUPLJANJE
    int brojPotiona;
    int brojKolacica;

    //REFERENCE
    GameManager zivotUI;
    GameManager potioniUI;
    GameManager kolaciciUI;

    private void Start()
    {
        trenutniZivot = maxZivot;
        ovisnost = 0;
        brzinaKretanja = defaultKretanje;
        jacinaSkoka = defaultSkok;
        anim_mac = FindObjectOfType<Animator>();
        zivotUI = FindObjectOfType<GameManager>();
        zivotUI.currentHP.text = trenutniZivot.ToString();
        brojPotiona = 0;
        brojKolacica = 0;
        potioniUI = FindObjectOfType<GameManager>();
        kolaciciUI = FindObjectOfType<GameManager>();
        potioniUI.brojPotiona.text = brojPotiona.ToString();
        kolaciciUI.brojKolacica.text = brojKolacica.ToString();
    }

    private void Update()
    {
        CameraView();
        EfektDroge();
        DelayCalculator();
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
        if(Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector3.forward * brzinaKretanja * Time.deltaTime); //LIJEVO
        }
        if(Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector3.back * brzinaKretanja * Time.deltaTime); //DESNO
        }
        if(Input.GetKeyDown(KeyCode.W) && isGrounded == true) //TREBA NARIKTATI DA SE SAMO JEDNOM MOŽE SKOČITI
        {
            GetComponent<Rigidbody>().velocity += Vector3.up * jacinaSkoka; //SKOK dodaje rigidbodyiju velocity prema gore za 7 jedinica
            isGrounded = false;
            jumpDelayActive = true;
        }
        if(Input.GetKeyDown(KeyCode.W) && uZraku == true && isGrounded == false)
        {
            GetComponent<Rigidbody>().velocity += Vector3.up * jacinaSkoka;
            uZraku = false;
        }
        if(Input.GetKeyDown(KeyCode.J)) 
        {
            anim_mac.Play("MacNapad");
        }
    }
    void DelayCalculator()
    {
        if(jumpDelayActive)
        {
            jumpDelay += Time.deltaTime;
            if(jumpDelay > 0.15)
            {
                uZraku = true;
                jumpDelay = 0;
                jumpDelayActive = false;
            }
        }
        if(imunityDelay > 0)
        {
            imunityDelay -= Time.deltaTime;
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
        if(isGrounded == false)
        {
            isGrounded = true;
            uZraku = false;
        }
        if(other.gameObject.tag == "Enemy" && imunityDelay <= 0)
        {
            trenutniZivot -= 10;
            zivotUI.currentHP.text = trenutniZivot.ToString();
            imunityDelay = 2;
        }
        if(other.gameObject.tag == "Potion")
        {
            brojPotiona++;
            potioniUI.brojPotiona.text = brojPotiona.ToString();
            Destroy(other.gameObject);
        }
        if (other.gameObject.tag == "Kolacic")
        {
            brojKolacica++;
            kolaciciUI.brojKolacica.text = brojKolacica.ToString();
            Destroy(other.gameObject);
        }
    }
}

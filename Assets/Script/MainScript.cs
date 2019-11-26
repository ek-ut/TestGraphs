
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Item
    {
    public GameObject Dot;
    public Clik ClikDot;
    }


public class MainScript : MonoBehaviour // main stage play class
{
    public GameObject ObDot;
    private MathsScript MatScr;
    private int maxDot = 6; //the number of points that the user sets
    private ArrayList Dotes = new ArrayList();
    int resolution = 1000; //количество точек в графике
    private ParticleSystem.Particle[] points; // number of points in the graph
    ParticleSystem ParSys;
    int power = 4;// degree for a least squares polynomial


    Item CreateItem() // adds a new point to the scene
    {
        
        float renX =  Random.Range(-9, 9);
        float renY =  Random.Range(-9, 9);
        return CreateItem(renX, renY);
    }

    Item CreateItem(float X, float Y) // adds a new point to the scene
    {
        Item NewItem;
        NewItem.Dot = Instantiate(ObDot, new Vector3(X, Y, 0f), Quaternion.identity);
        NewItem.ClikDot = NewItem.Dot.GetComponent<Clik>();
        return NewItem;
    }


    void AddItems(Item NewItem) // adds a NewItem element to a sorted Dotes array
    {
        int sizeArray = Dotes.Count;
        Item CounterItem;
        Vector3 pozCoutent;
        Vector3 pozNewItem = NewItem.Dot.transform.position;
        bool add = false;

        for (int i= 0; i< sizeArray; i++)
            {
            CounterItem = (Item)Dotes[i];
            pozCoutent = CounterItem.Dot.transform.position;
            if (pozNewItem.x < pozCoutent.x)
            {
                Dotes.Insert(i, NewItem);
                add = true;
                break;
            }
        }
        if(!add)
        {
            Dotes.Add(NewItem);
        }
        
    }

    

    float[] MNK(int power) //creating a math matrix
    {
        int sizeArrey = Dotes.Count;
        float[] x = new float[sizeArrey];
        float[] y = new float[sizeArrey];
        Item CounterItem;
        Vector3 pozCoutent;

        for (int i = 0; i< sizeArrey; i++)
        {
            CounterItem = (Item)Dotes[i];
            pozCoutent = CounterItem.Dot.transform.position;
            x[i] = pozCoutent.x;
            y[i] = pozCoutent.y;
        }

        return MatScr.MNK(power, x,y);
    }

    void MNKCreation()// creating graph least squares method
    {
        
        float[] param = MNK(power);
        if (param != null)
        {
            ArrayList Al = new ArrayList();
            for (int i = 0; i <= power; i++)
            {
                while (((param[i] > 0) && (param[i] < 1)) || ((param[i] < 0) && (param[i] > -1)))
                {
                    param[i] = param[i] * 10;
                }
                Al.Add(param[i]);
                
            }
            ScheduleCreationMPC(Al);
        }
        
    }

    void ScheduleCreationMPC(ArrayList a)// draws graph least squares method
    {
        
        int tugraf = resolution / 2; 
        float increment = 64f / (resolution - 1);
        float yf;
        for (int i = 0; i < tugraf; i++)
        {
            float xf = i * increment - 15;
            yf = MatScr.GetYfromX(xf, a)/10;
            
            points[i].position = new Vector3(xf, yf, 0f);
            points[i].startColor = new Color(255, 0, 0);
            points[i].startSize = 0.5f;
        }
    }

    void ScheduleCreationPL()// creating graph PL
    {
        int sizeArrey = Dotes.Count;
        float[] x = new float[sizeArrey];
        float[] y = new float[sizeArrey];
        Item CounterItem;
        Vector3 pozCoutent;

        for (int i = 0; i < sizeArrey; i++)
        {
            CounterItem = (Item)Dotes[i];
            pozCoutent = CounterItem.Dot.transform.position;
            x[i] = pozCoutent.x;
            y[i] = pozCoutent.y;
        }
        int tugraf = resolution / 2;
        float increment = 64f / (resolution - 1);
        for (int i = tugraf; i < resolution; i++)
        {
            float Xcf = (i - tugraf) * increment - 15;
            points[i].position = new Vector3(Xcf, MatScr.LagrangePolynomial(x,y,Xcf), 0f);
            points[i].startColor = new Color(255, 255, 0);
            points[i].startSize = 0.5f;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        points = new ParticleSystem.Particle[resolution]; 
        MatScr = GetComponent<MathsScript>();
        ParSys = GetComponent<ParticleSystem>();
    }

    bool CheckChengeDot()// check whether the state of the points has changed
    {
        int size = Dotes.Count;
        Item CounterItem;
        bool Chenge = false;
        for (int i = 0; i < size; i++)
        {
            CounterItem = (Item)Dotes[i];
            Chenge = Chenge || CounterItem.ClikDot.Chenge;
            CounterItem.ClikDot.Chenge = false;
        }
        return Chenge;
    }
    void Upd() // refresh scene
    {
        MNKCreation();
        ScheduleCreationPL();
        ParSys.SetParticles(points, points.Length);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) ) 
        {
            if(Dotes.Count < maxDot)  AddItems(CreateItem());
            
        }
        if ((Input.touchCount > 0) )
        {
            if (Dotes.Count < maxDot)
            {
                Vector2 TachPos = Input.GetTouch(0).position;
                AddItems(CreateItem(TachPos.x, TachPos.y));
            }
        }
        if (CheckChengeDot()) Upd();
    }
}

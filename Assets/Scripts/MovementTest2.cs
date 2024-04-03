using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementTest2 : MonoBehaviour
{
    public float moveSpeed;
    public float rotSpeed;
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        //Variável local de vetor de velocidade
        float vertical;

        //Se teclas A e D estiverem sendo seguradas ao mesmo tempo, personagem desacelera. Senão, segue em frente com velocidade normal
        if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D)) vertical = 0;
        else vertical = 1;

        //Variável local de valor de rotação a adicionar
        float rotation = Input.GetAxis("Horizontal") * rotSpeed * Time.deltaTime;

        //Se teclas A e D estiverem sendo seguradas ao mesmo tempo, personagem para de girar para os lados.
        if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D)) rotation = 0;
        
        transform.eulerAngles += new Vector3(0, rotation, 0);

        Vector3 direction = new Vector3(0, 0, vertical);

        direction = transform.TransformDirection(direction);
        rb.AddForce(direction * moveSpeed);
    }
}

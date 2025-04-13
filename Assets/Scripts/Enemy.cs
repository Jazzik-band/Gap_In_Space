using System;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.Serialization;

public class Enemy: MonoBehaviour
{
    public float enemySpeed;
    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    private void Update()
    {
        if (Vector2.Distance(player.transform.position, transform.position) <= 7)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, enemySpeed * Time.deltaTime);
        }
    }
}

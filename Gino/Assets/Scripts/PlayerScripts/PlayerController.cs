﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Information")]
    public float speed = 50f;
    public float maxSpeed = 3;
    public float jumpPow = 800f;
    public float maxJumpPow = 200f;
    
    [Header("Condition")]
    public bool grounded = true;
    public bool faceRight = true;

    [Header("Heath")]
    public int ourHealth;
    public int maxHealth = 6;

    private Rigidbody2D r2;
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {

        r2 = gameObject.GetComponent<Rigidbody2D>();
        anim = gameObject.GetComponent<Animator>();
        ourHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetBool("Grounded", grounded);
        anim.SetFloat("Run", Mathf.Abs(r2.velocity.x));
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (grounded)
            {
                FindObjectOfType<SoundManager>().Play("Jump");
                grounded = false;
                r2.AddForce(Vector2.up * jumpPow);
            }     
        }
    }

    void FixedUpdate()
    {
        // Moving
        float horizontal = Input.GetAxis("Horizontal");
        r2.AddForce((Vector2.right) * speed * horizontal);

        // Trả giá trị lực đẩy về maxSpeed khi vượt quá maxSpeed--------
        if (r2.velocity.x > maxSpeed)
        {
            r2.velocity = new Vector2(maxSpeed, r2.velocity.y);
        }
        if (r2.velocity.x < -maxSpeed)
        {
            r2.velocity = new Vector2(-maxSpeed, r2.velocity.y);
        }
        //---------------------------------------------------------
        // Trả giá trị lực đẩy lên về maxJump khi vượt quá maxJump--------
        if (r2.velocity.y > maxJumpPow)
        {
            r2.velocity = new Vector2(r2.velocity.x, maxJumpPow);
        }
        if (r2.velocity.y < -maxJumpPow)
        {
            r2.velocity = new Vector2(r2.velocity.x, -maxJumpPow);
        }
        //---------------------------------------------------------
        if (horizontal>0 && !faceRight)
        {
            Flip();
        }
        if (horizontal < 0 && faceRight)
        {
            Flip();
        }
        // Giam speed ko bi truot
        if (grounded)
        {
            r2.velocity = new Vector2(r2.velocity.x * 0.7f, r2.velocity.y);
        }
        // Máu <= 0 player sẽ chết
        if(ourHealth <= 0)
        {
            Death();
        }
    }
    // Lật ngược hình nhân vật
    public void Flip()
    {
        faceRight = !faceRight;
        Vector3 scale;
        scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
    // Giảm máu tương đương lượng damage chuyền vào
    public void Damage(int damage)
    {
        FindObjectOfType<SoundManager>().Play("Dead");
        ourHealth -= damage;
        gameObject.GetComponent<Animation>().Play("Player_TakeDmg");
    }
    // Dead
    public void Death()
    {
        anim.SetBool("Dead", true);
        StartCoroutine(DeadDelay());
    }
    IEnumerator DeadDelay()
    {
        yield return new WaitForSeconds(0.8f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    //-----------------
    // Sinh ra lực đẩy với giá trị truyền vào 
    public void KnockBack(float KnockPow, float KnockTrend, Vector2 KnockDir)
    {
        r2.velocity = new Vector2(0, 0);
        r2.AddForce(new Vector2(KnockDir.x * KnockTrend, KnockDir.y * KnockPow));
    }

}

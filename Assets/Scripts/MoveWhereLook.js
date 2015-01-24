#pragma strict

function Start () {

}

function Update () {
var speed: float= 3.0;
var con = GetComponent(CharacterController);
var dir = con.transform.forward;

dir.z *= speed;
con.Move(dir * Time.deltaTime);
}
///<summary>
/// PlayerControl is a simple solution to control the character.
/// It uses the Horizontal and Vertical axis from the InputManager. 
/// PlayerControl will change the animation of the character using Mecanim animation state machine.
///</summary>
///<version>
/// 1.0, 2014.01.22 by Attila Odry
///</version>
///<remarks>
/// Copyright (c) 2013, Xsens Technologies B.V.
/// All rights reserved.
/// 
/// Redistribution and use in source and binary forms, with or without modification,
/// are permitted provided that the following conditions are met:
/// 
///  - Redistributions of source code must retain the above copyright notice, 
///        this list of conditions and the following disclaimer.
///  - Redistributions in binary form must reproduce the above copyright notice, 
///    this list of conditions and the following disclaimer in the documentation 
///    and/or other materials provided with the distribution.
/// 
/// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
/// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
/// AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS
/// BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES 
/// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, 
/// OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
/// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, 
/// EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
///</remarks>

using UnityEngine;
using System.Collections;

namespace xsens
{

	public class PlayerControl : MonoBehaviour {

		public float maxMove = 0.1f;		///limitation for maximum move per update
		
		private Animator animator;			///Connection to Mecanim state machine
		private float lastV;				///last vertical value (forward)
		private float lastH;				///last horizontal value (sideways)
		private float speed;				///forward speed (0:stop, 1:run)
		private float direction;			///turning direction (0:straight, 1:right, -1:left)

		// Use this for initialization
		void Start () {
		
			//find Mecanim Animator within the character
			animator = GetComponent<Animator>();
			if (animator == null) Debug.LogError("No animator found:"+gameObject.name);

			//reset all values
			lastV = 0;
			lastH = 0;
			speed = 0;
			direction = 0;

		}//Start()
		
		// Update is called once per frame
		void Update () {

			//get user input
			float h = Input.GetAxis("Horizontal"); 	//direction
			float v = Input.GetAxis("Vertical");	//speed

			//don't allow to quick moves
			if(v > lastV+maxMove)
				v = lastV + maxMove;
			else if(v < lastV-maxMove)
				v = lastV - maxMove;
			if(h > lastH+maxMove)
				h = lastH + maxMove;
			else if(h < lastH-maxMove)
				h = lastH - maxMove;		

			//get speed and direction
			speed = h*h+v*v;	
			direction = h;

			//apply speed only if we have vertical speed
			if(v == 0) speed = 0;

			animator.SetFloat("Speed", speed);
			animator.SetFloat("Direction", direction);

			//save last values
			lastV = v;
			lastH = h;

		}//Update()

	}//class PlayerControl
}//namespace xsens
﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Evol.Game.Player;
using Evol.Heuristic.StateMachine;
using Evol.Utils;
using Photon.Pun;
using TMPro;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace Evol.Game.Misc
{
    public class GameController : MonoBehaviour
    {
        // Lets make a list with constant int linked to gameobject ?
        public List<CharacterData> Characters;

        public GameObject Wolf; // TODO: think about a clean way to store some spawnable prefabs like monsters ... ?
        
        private enum GameState
        {
            Playing,
            Won,
            Lost
        }

        private GameState gameState = GameState.Playing;

        // Start is called before the first frame update
        void Start()
        {
            // Retrieve the chosen character
            var characterId = PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("character") ?
                Convert.ToInt32(PhotonNetwork.LocalPlayer.CustomProperties["character"]) :
                throw new Exception($"The player has no character in his properties !");
            
            // Instanciate the player
            PhotonNetwork.Instantiate(Characters
                .Find(c => c.Id == characterId).Prefab.name,
                Position.RandomPositionAround(Vector3.zero, 30),
                Quaternion.identity);
            
            if (PhotonNetwork.IsMasterClient)
            {
                foreach(var i in Enumerable.Range(0, 10))
                {
                    var go = PhotonNetwork.Instantiate(Wolf.name,
                        Position.AboveGround(Position.RandomPositionAround(Vector3.zero, 200),
                            Wolf.GetComponent<Collider>().bounds.size.y),
                        Quaternion.identity);
                    go.GetComponent<StateController>().SetupAi(true, new List<Transform>());
                }

                StartCoroutine(GameLoop());
            }
        }

        // This is called from start and will run each phase of the game one after another.
        private IEnumerator GameLoop()
        {

            // Start off by running the 'GameStarting' coroutine but don't return until it's finished.
            yield return StartCoroutine(GameStarting());
            
            // Once the 'GameStarting' coroutine is finished, run the 'RoundPlaying' coroutine but don't return until it's finished.
            yield return StartCoroutine(GamePlaying());

            // Once execution has returned here, run the 'GameEnding' coroutine, again don't return until it's finished.
            yield return StartCoroutine(GameEnding());
            
            // Should run this in a loop ?
            // This code is not run until 'RoundEnding' has finished.  At which point, check if a game winner has been found.
            switch (gameState)
            {
                case GameState.Won:
                    // If there is a game winner, what are we doing ?
                    // Show stats idk
                    // Send RPC asking to go back to main menu to everyone
                    break;
                case GameState.Lost:
                    // If game is lost, what are we doing ?
                    // Show stats idk
                    // Send RPC asking to go back to main menu to everyone
                    break;
                case GameState.Playing:
                    // If there isn't a winner yet, restart this coroutine so the loop continues.
                    // Note that this coroutine doesn't yield.  This means that the current version of the GameLoop will end.
                    // StartCoroutine(GameLoop());
                    break;
            }
        }

        private IEnumerator GameStarting()
        {
            // photonView.RPC("UpdateText", RpcTarget.All, "Waiting more players or press Space to play solo");
            // mainText.text = "Press space to start";
            // Wait other players and that everyone is ready, we also check if nobody has every joined
            /*
            while (nobodyJoinedYet && 
                   (PhotonNetwork.PlayerList.Count(p => p.CustomProperties.ContainsKey("ready") && p.CustomProperties["ready"].Equals("true"))
                   < PhotonNetwork.CountOfPlayers))
            {
                yield return null;
            }*/
            
            gameState = GameState.Playing;
            // photonView.RPC("UpdateText", RpcTarget.All, "Kill them all");
            // mainText.text = "Game starting";
            print("Game starting");

            // Wait for the specified length of time until yielding control back to the game loop.
            yield return new WaitForSeconds(2);
        }


        private IEnumerator GamePlaying()
        {
            while (gameState == GameState.Playing)
            {
                yield return null;
                /*
                // Start off by running the 'RoundStarting' coroutine but don't return until it's finished.
                yield return StartCoroutine(RoundStarting());

                // ... return on the next frame.
                yield return StartCoroutine(RoundPlaying());
                if (GameFinished())
                    yield break;

                yield return StartCoroutine(RoundEnding());
                */
            }
        }

        private IEnumerator GameEnding()
        {
            // Stop from moving.
            // DisableControl();
            
            if (gameState == GameState.Lost)
            {
                // mainText.text = "GAME OVER";
                print("Game over");
            }
            
            if(gameState == GameState.Won)
            {
                // mainText.text = "VICTORY";
                print("Victory");
            }
            
            // Wait for the specified length of time until yielding control back to the game loop.
            yield return new WaitForSeconds(2);
        }
    }
}
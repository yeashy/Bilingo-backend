﻿using Bilingo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bilingo.Controllers
{
    [ApiController]
    public class WordsController : Controller
    {
        private readonly IWordsService _wordsService;
        private readonly ILoginService _loginService;

        public WordsController(IWordsService wordsService, ILoginService loginService)
        {
            _wordsService = wordsService;
            _loginService = loginService;
        }

        [HttpPost]
        [Route("initWords")]
        public async Task<IActionResult> Post()
        {
            try
            {
                await _wordsService.InitWords();
                return Ok(new { message = "OK" });
            }
            catch (Exception ex)
            {
                var response = new
                {
                    message = ex.InnerException == null ? ex.Message : ex.InnerException.Message
                };
                return StatusCode(500, response);
            }
        }

        [HttpPost]
        [Authorize]
        [Route("setAlreadyKnown/{wordId:int}")]
        public async Task<IActionResult> SetAlreadyKnown(int wordId)
        {
            string usernameClaim = User.Claims.ToList()[0].ToString();
            string username = usernameClaim.Substring(usernameClaim.IndexOf(" ") + 1);
            var userId = await _loginService.GetIdByUsername(username);
            if (userId == null)
            {
                return StatusCode(500, new { message = "User wasn't found" });
            }
            try
            {
                await _wordsService.SetAlreadyKnown((int)userId, wordId);
                return Ok(new { message = "OK" });    
            }
            catch (Exception ex)
            {
                var response = new
                {
                    message = ex.InnerException == null ? ex.Message : ex.InnerException.Message
                };
                return StatusCode(500, response);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("getNewWord")]
        public async Task<IActionResult> GetNewWord()
        {
            string usernameClaim = User.Claims.ToList()[0].ToString();
            string username = usernameClaim.Substring(usernameClaim.IndexOf(" ") + 1);
            var userId = await _loginService.GetIdByUsername(username);
            if (userId == null)
            {
                return StatusCode(500, new { message = "User wasn't found" });
            }
            try
            {
                var res = await _wordsService.GetNewWord((int)userId);
                return new JsonResult(res);
            }
            catch (Exception ex)
            {
                var response = new
                {
                    message = ex.InnerException == null ? ex.Message : ex.InnerException.Message
                };
                return StatusCode(500, response);
            }
        }

        [HttpPost]
        [Authorize]
        [Route("switchToNewStage/{wordId:int}")]
        public async Task<IActionResult> SwitchToNewStage(int wordId)
        {
            string usernameClaim = User.Claims.ToList()[0].ToString();
            string username = usernameClaim.Substring(usernameClaim.IndexOf(" ") + 1);
            var userId = await _loginService.GetIdByUsername(username);
            if (userId == null)
            {
                return StatusCode(500, new { message = "User wasn't found" });
            }
            try
            {
                await _wordsService.SwitchToNewStage(wordId, (int)userId);
                return Ok(new { message = "OK" });
            }
            catch (Exception ex)
            {
                var response = new
                {
                    message = ex.InnerException == null ? ex.Message : ex.InnerException.Message
                };
                return StatusCode(500, response);
            }
        }


        // TODO:
        // 1. get new word to learn [DONE]
        // 2. get word to repeat
        // 3. set already known [DONE]
        // 4. switch to new stage [DONE]
        // 5. Game1. Get sentence.
        // 6. Game2. Listen to sentence and type it (will be implemented on front). Basycally it's the same method as the previos one
        // 7. Game3. Choose right definition of word (need to provide 4 definitions)
        // 8. Game4. Choose right syn/ant for the word (need to provide 4 aswell)
        // 9. Get stat and progress
        // 10. Get info about a word (not sure should we do it)
    }
}

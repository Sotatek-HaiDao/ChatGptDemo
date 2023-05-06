
# ChatGptDemo




## API Reference

### Send Question

```http
  POST ~/api/Chat
```

| Parameter | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `message` | `string` | **Required**. User Input Question |
| `chatHistory` | `string[]` | **Required**. Chat History of current session |

#### Notes
- For first question, chatHistory = [message_input]
- For 2nd question , chatHistory = [First_question_input, first_response_output]
- For 3th question, chatHistory = [...,second_question_input, second_response_output]
- And so on...

#### Response 

| Parameter | Type     | Description                       |
| :-------- | :------- | :-------------------------------- |
| `Role`    | `string` |
| `Content` | `string` |Response from chatGPT|



<div align=center>
    <h1>RP Weave</h1>
</div>

---

RP Weave is a free and open-source platform for TTRPG world-building.

### Features

RP Weave currently has the following features:
- Creating a campaign with optional text material submission
- PDF and Markdown text materials support
- Chat with an AI model about the materials (currently without persistence)

### Getting started

Two images exist for RP Weave, one for front-end and one for back-end. Additionally, RP Weave makes use of other open-source services. A demo Docker Compose file is included in the "demo" directory, which can be adjusted to your preferences.

Quick step-by-step without diving too deep:
- Copy the contents of the /demo/docker-compose.demo.yml and .env
- Adjust container names and parameter values in the .env file to your preference
- Start the containers to get initial setup
- To use Ollama, you need at least two models:
    - An embedding model, it will be used for creating vectors off of your text and queries so that searches can be done
    - A reasoning model, the one that will do heavier thinking and responding to chat
- Head over to https://ollama.com/search and find appropriate embedding and reasoning models, you can always change which ones you use later
- Run ```docker exec -it <ollama_container_name> bash``` and run ```ollama pull <model_name>``` for each model of your choice
- Going to the UI, you can login with the initial admin account with username "admin" and password "ChangeMe!123". Don't forget to change it later ;)

For more in-depth information about each other service, head over to their respective documentation pages.

### Development

RP Weave is still in early development with no stable version present and no timeline. The images tagged with "edge" are the latest versions, containing the latest features.

As the project matures, breaking changes may occur. 

If any interest in this project is sparked, better communication methods and roadmaps will be established.

### Feedback

Feedback is always appreciated, from code to infrastructure or documentation. Feel free to open issues. However, a timeline with fixes to any potential issues cannot be provided.

### Licensing

The code in this project is licensed under GPL-3.0 license.
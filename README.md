DOCUMENTATION DEVOPS



Pipeline CI/CD — Du Code au Déploiement en Production

Environnement
Serveur : Windows Server 2022  |  Clients : Windows 10/11
Outils : Git, Jenkins, Docker, Kubernetes, Grafana, Prometheus

1. Code
2. Commit
3. Build CI
4. Docker
5. Deploy CD
6. Monitor


Mars 2026

Table des Matières

Introduction au DevOps3
Prérequis & Installation de l'environnement4
Étape 1 — Code (Environnement de développement)6
Étape 2 — Commit (Git & GitHub/GitLab)8
Étape 3 — Build CI (Jenkins)11
Étape 4 — Docker Image (Conteneurisation)15
Étape 5 — Deploy CD (Kubernetes / Déploiement)19
Étape 6 — Monitor (Grafana, Prometheus, ELK)23
Pipeline Complet — Résumé et Bonnes Pratiques27
Annexes & Commandes Essentielles29

Introduction au DevOps
Le DevOps est une culture et un ensemble de pratiques qui unifient le développement logiciel (Dev) et les opérations informatiques (Ops). L'objectif est de raccourcir le cycle de vie du développement tout en livrant des fonctionnalités, correctifs et mises à jour en continu et de haute qualité.

Pourquoi le DevOps ?
Livraisons plus rapides et plus fiables
Détection précoce des bugs grâce aux tests automatisés
Collaboration améliorée entre développeurs et administrateurs
Infrastructure reproductible et traçable
Réduction des risques lors des mises en production

Le Pipeline DevOps en 6 Étapes

#
Étape
Description
1
Code
Les développeurs écrivent et testent le code localement dans leur IDE (VS Code, IntelliJ, PyCharm…)
2
Commit
Le code est poussé vers un dépôt Git (GitHub/GitLab) pour la collaboration et le suivi des modifications
3
Build (CI)
Des outils comme Jenkins ou GitLab CI compilent l'application, installent les dépendances et lancent les tests automatiquement
4
Docker Image
L'application est empaquetée dans un conteneur Docker pour garantir un comportement identique quel que soit l'environnement
5
Deploy (CD)
Le conteneur est déployé sur Kubernetes, des serveurs cloud ou d'autres plateformes de manière automatisée
6
Monitor
Des outils comme Grafana, Prometheus et ELK Stack surveillent les performances et les logs en temps réel


Contexte : Windows Server
Ce guide est adapté à un environnement Windows Server 2022 pour le serveur de production et des postes clients Windows 10/11.
Toutes les configurations et commandes sont données pour cet environnement.
Certains outils (Docker, Kubernetes, Jenkins) ont des spécificités Windows qui seront détaillées dans chaque section.


Prérequis & Installation de l'Environnement
Avant de commencer, il est indispensable de mettre en place l'environnement de base sur votre serveur Windows Server 2022 et vos machines clientes.

Configuration du Serveur Windows Server 2022
Spécifications Matérielles Recommandées
Composant
Minimum
Recommandé
CPU
4 cœurs
8 cœurs ou plus
RAM
8 GB
16 GB ou plus
Stockage
100 GB SSD
500 GB SSD
Réseau
100 Mbps
1 Gbps
OS
Windows Server 2022
Windows Server 2022 Datacenter


Activation de Hyper-V (pour Docker Windows)
Sur Windows Server, ouvrez PowerShell en tant qu'Administrateur et exécutez :
# Activer Hyper-V
Install-WindowsFeature -Name Hyper-V -IncludeManagementTools -Restart


# Vérifier que Hyper-V est actif
Get-WindowsOptionalFeature -Online -FeatureName Microsoft-Hyper-V

Activation des Conteneurs Windows
# Activer la fonctionnalité Conteneurs
Install-WindowsFeature -Name Containers -Restart


# Vérifier l'activation
Get-WindowsFeature -Name Containers

Logiciels à Installer sur le Serveur
Logiciel
Version
Rôle
Git for Windows
2.x ou plus
Gestion du code source
Java JDK 17 (LTS)
17+
Requis par Jenkins
Jenkins
LTS 2.x
Serveur CI/CD
Docker Desktop
4.x
Conteneurisation
kubectl
1.28+
CLI Kubernetes
Node.js (si besoin)
LTS 20.x
Pour apps JavaScript
Prometheus
2.x
Collecte de métriques
Grafana
10.x
Tableaux de bord


Configuration du Pare-feu Windows
Ouvrir les ports nécessaires sur le serveur pour permettre la communication entre les services :
# Jenkins (port 8080)
New-NetFirewallRule -DisplayName 'Jenkins' -Direction Inbound -Protocol TCP -LocalPort 8080 -Action Allow


# Docker (port 2376)
New-NetFirewallRule -DisplayName 'Docker' -Direction Inbound -Protocol TCP -LocalPort 2376 -Action Allow


# Grafana (port 3000)
New-NetFirewallRule -DisplayName 'Grafana' -Direction Inbound -Protocol TCP -LocalPort 3000 -Action Allow


# Prometheus (port 9090)
New-NetFirewallRule -DisplayName 'Prometheus' -Direction Inbound -Protocol TCP -LocalPort 9090 -Action Allow


# Kubernetes API (port 6443)
New-NetFirewallRule -DisplayName 'Kubernetes API' -Direction Inbound -Protocol TCP -LocalPort 6443 -Action Allow

ÉTAPE 1
CODE — Environnement de Développement


La première étape du pipeline DevOps est la phase de développement. Les développeurs écrivent, testent et déboguent leur code sur leur machine locale avant de le soumettre au dépôt.

Outils de Développement (IDE)
Un IDE (Integrated Development Environment) est l'éditeur de code principal du développeur. Voici les plus courants :
IDE
Langages
Avantages
VS Code
Tous (universel)
Léger, extensible, gratuit, extensions DevOps
IntelliJ IDEA
Java, Kotlin, Python
Puissant, autocomplétion avancée
PyCharm
Python
Spécialisé Python, débogueur intégré
Eclipse
Java, C++


Configuration de VS Code pour DevOps
VS Code est recommandé pour ce projet. Installez les extensions suivantes pour un workflow DevOps efficace :
# Installer VS Code extensions en ligne de commande
code --install-extension ms-vscode-remote.remote-ssh
code --install-extension ms-azuretools.vscode-docker
code --install-extension ms-kubernetes-tools.vscode-kubernetes-tools
code --install-extension eamodio.gitlens
code --install-extension hashicorp.terraform
code --install-extension redhat.vscode-yaml

Structure d'un Projet DevOps
Un projet bien structuré facilite l'automatisation. Voici l'arborescence recommandée :
mon-projet/
├── src/                   # Code source de l'application
│   └── app.js (ou main.py, etc.)
├── tests/                 # Tests automatisés
│   └── test_app.js
├── Dockerfile             # Instructions de conteneurisation
├── docker-compose.yml     # Configuration multi-conteneurs
├── Jenkinsfile            # Pipeline CI/CD Jenkins
├── kubernetes/            # Configurations Kubernetes
│   ├── deployment.yaml
│   └── service.yaml
├── monitoring/            # Configuration monitoring
│   └── prometheus.yml
├── .gitignore             # Fichiers à exclure de Git
└── README.md              # Documentation du projet

Écriture des Tests — Bonne Pratique Essentielle
En DevOps, le code doit toujours être accompagné de tests. Sans tests, l'automatisation CI/CD ne peut pas valider que votre code fonctionne correctement.
Exemple de test simple (JavaScript / Jest)
// tests/test_app.js
const { add } = require('../src/app');


test('addition de 2 + 3 retourne 5', () => {
  expect(add(2, 3)).toBe(5);
});


test('addition de nombres négatifs', () => {
  expect(add(-1, -1)).toBe(-2);
});

Bonne Pratique : Convention de Nommage
• Nommez clairement vos branches Git : feature/nom-fonctionnalite, bugfix/description, hotfix/urgent
• Écrivez des messages de commit descriptifs (voir étape 2)
• Chaque fonctionnalité doit avoir au minimum un test unitaire
• Ne commitez jamais de mots de passe, clés API ou données sensibles directement dans le code


ÉTAPE 2
COMMIT — Git & Gestion du Code Source


Git est le système de contrôle de version le plus utilisé au monde. Il permet à une équipe de travailler sur le même code sans se marcher dessus, et conserve l'historique complet de toutes les modifications.

Installation et Configuration de Git sur Windows
Installation
Téléchargez Git for Windows depuis https://git-scm.com et installez-le avec les options par défaut.

Configuration initiale (obligatoire)
# Configurer votre identité Git (à faire une seule fois)
git config --global user.name "Votre Nom"
git config --global user.email "votre@email.com"


# Configurer l'éditeur par défaut
git config --global core.editor "code --wait"


# Configurer la branche principale par défaut
git config --global init.defaultBranch main


# Vérifier la configuration
git config --list

Commandes Git Fondamentales
Commande
Description
git init
Initialise un nouveau dépôt Git dans le dossier courant
git clone <url>
Copie un dépôt distant sur votre machine
git status
Affiche l'état des fichiers (modifiés, ajoutés, etc.)
git add <fichier>
Ajoute un fichier à la zone de staging
git add .
Ajoute tous les fichiers modifiés
git commit -m 'message'
Enregistre les modifications avec un message
git push origin main
Envoie les commits vers le dépôt distant
git pull
Récupère et fusionne les dernières modifications
git branch nom-branche
Crée une nouvelle branche
git checkout nom-branche
Change de branche
git merge nom-branche
Fusionne une branche dans la branche courante
git log --oneline
Affiche l'historique des commits


Workflow Git Recommandé (Git Flow)
Le Git Flow est une stratégie de branches qui organise le développement en équipe de manière claire et sécurisée.
# 1. Cloner le projet
git clone https://github.com/organisation/mon-projet.git
cd mon-projet


# 2. Créer une branche pour votre fonctionnalité
git checkout -b feature/nouvelle-fonctionnalite


# 3. Faire vos modifications, puis les enregistrer
git add .
git commit -m "feat: ajout de la fonctionnalité de connexion"


# 4. Pousser la branche vers GitHub
git push origin feature/nouvelle-fonctionnalite


# 5. Créer une Pull Request sur GitHub/GitLab pour code review
# (à faire via l'interface web)


# 6. Après approbation, merger dans main
git checkout main
git merge feature/nouvelle-fonctionnalite
git push origin main

Configuration SSH pour GitHub/GitLab
Configurer SSH évite de saisir votre mot de passe à chaque push/pull.
# Générer une clé SSH sur votre machine Windows
ssh-keygen -t ed25519 -C "votre@email.com"
# Appuyez sur Entrée pour accepter le chemin par défaut
# Vous pouvez ajouter une passphrase pour plus de sécurité


# Afficher la clé publique à copier dans GitHub/GitLab
type C:\Users\VotreNom\.ssh\id_ed25519.pub


# Tester la connexion
ssh -T git@github.com
# Résultat attendu : Hi username! You've successfully authenticated...

Fichier .gitignore — Ce qu'il ne faut jamais committer
Le fichier .gitignore liste les fichiers que Git doit ignorer. À placer à la racine du projet :
# Fichier .gitignore


# Dépendances
node_modules/
vendor/
__pycache__/
*.pyc


# Variables d'environnement et secrets
.env
.env.local
*.key
*.pem
config/secrets.yml


# Fichiers IDE
.vscode/settings.json
.idea/
*.suo


# Fichiers OS
Thumbs.db
.DS_Store


# Fichiers de build
dist/
build/
*.log

ÉTAPE 3
BUILD CI — Jenkins & Intégration Continue


L'Intégration Continue (CI) est le processus qui automatise la vérification du code à chaque push. Jenkins surveille votre dépôt Git et déclenche automatiquement : la compilation, l'installation des dépendances et l'exécution des tests.

Installation de Jenkins sur Windows Server
1. Installer Java JDK 17 (prérequis)
# Télécharger Java JDK 17 depuis https://adoptium.net
# Ou via winget (Windows Package Manager)
winget install EclipseAdoptium.Temurin.17.JDK


# Vérifier l'installation
java -version
# Résultat attendu : openjdk version '17.x.x' ...

2. Installer Jenkins
# Télécharger Jenkins LTS depuis https://www.jenkins.io/download/
# Choisir 'Windows' et télécharger le fichier .msi


# Jenkins s'installe en tant que service Windows automatiquement
# Vérifier que le service tourne
Get-Service -Name Jenkins


# Démarrer/Redémarrer Jenkins si nécessaire
Restart-Service -Name Jenkins


# Jenkins sera accessible sur : http://VOTRE-IP-SERVEUR:8080

3. Configuration initiale de Jenkins
Lors de la première connexion à http://VOTRE-IP:8080, Jenkins demande un mot de passe initial :
# Récupérer le mot de passe initial de Jenkins
type C:\ProgramData\Jenkins\.jenkins\secrets\initialAdminPassword


# Copier ce mot de passe et le coller dans l'interface web Jenkins
# Puis : Installer les plugins suggérés -> Créer un compte admin

Configuration du Pipeline Jenkins (Jenkinsfile)
Un Jenkinsfile définit le pipeline CI/CD de votre projet. Il doit être placé à la racine de votre dépôt Git.
// Jenkinsfile — Pipeline complet
pipeline {
    agent any


    environment {
        DOCKER_IMAGE = 'mon-app'
        DOCKER_TAG   = "${env.BUILD_NUMBER}"
        REGISTRY     = 'mon-registry.local'
    }


    stages {
        stage('Checkout') {
            steps {
                echo 'Recuperation du code source...'
                checkout scm
            }
        }


        stage('Installer les dependances') {
            steps {
                echo 'Installation des dependances...'
                bat 'npm install'   // Windows utilise 'bat' (pas 'sh')
            }
        }


        stage('Tests') {
            steps {
                echo 'Execution des tests...'
                bat 'npm test'
            }
            post {
                always {
                    junit 'test-results/**/*.xml'
                }
            }
        }


        stage('Build Docker Image') {
            steps {
                echo 'Construction de l image Docker...'
                bat "docker build -t ${DOCKER_IMAGE}:${DOCKER_TAG} ."
            }
        }


        stage('Push vers Registry') {
            steps {
                withCredentials([usernamePassword(
                    credentialsId: 'docker-registry-creds',
                    usernameVariable: 'DOCKER_USER',
                    passwordVariable: 'DOCKER_PASS'
                )]) {
                    bat "docker login ${REGISTRY} -u %DOCKER_USER% -p %DOCKER_PASS%"
                    bat "docker push ${REGISTRY}/${DOCKER_IMAGE}:${DOCKER_TAG}"
                }
            }
        }


        stage('Deploiement') {
            when { branch 'main' }
            steps {
                echo 'Deploiement en production...'
                bat "kubectl set image deployment/mon-app mon-app=${REGISTRY}/${DOCKER_IMAGE}:${DOCKER_TAG}"
            }
        }
    }


    post {
        success { echo 'Pipeline termine avec succes !' }
        failure { echo 'Echec du pipeline — verifiez les logs' }
    }
}

Connexion Jenkins à GitHub/GitLab
Dans Jenkins : Tableau de bord > Gérer Jenkins > Credentials
Ajouter un credential de type 'Username with password' ou 'SSH Username with private key'
Créer un nouveau Job Pipeline : Nouveau projet > Pipeline
Dans la configuration du job : cocher 'Poll SCM' ou configurer un Webhook GitHub
Dans 'Pipeline' : choisir 'Pipeline script from SCM' puis pointer vers votre dépôt

Important : Plugins Jenkins à Installer
• Git plugin — pour l'intégration avec les dépôts Git
• Docker Pipeline — pour les commandes Docker dans le Jenkinsfile
• Kubernetes plugin — pour le déploiement Kubernetes
• Blue Ocean — interface visuelle moderne pour les pipelines
• JUnit — pour afficher les résultats de tests


ÉTAPE 4
DOCKER IMAGE — Conteneurisation


Docker est une plateforme de conteneurisation qui permet d'empaqueter une application et toutes ses dépendances dans une unité standardisée appelée conteneur. Le conteneur fonctionnera de manière identique sur n'importe quelle machine.

Concepts Fondamentaux Docker
Concept
Définition
Image
Modèle immuable contenant le code, les dépendances et la configuration
Conteneur
Instance en cours d'exécution d'une image (comme un processus isolé)
Dockerfile
Fichier texte contenant les instructions pour construire une image
Registry
Dépôt d'images Docker (Docker Hub, registry privé)
Volume
Stockage persistant partagé entre le conteneur et l'hôte
Network
Réseau virtuel permettant aux conteneurs de communiquer


Installation de Docker sur Windows Server 2022
# Installer Docker Desktop pour Windows
# Télécharger depuis https://www.docker.com/products/docker-desktop


# Ou via PowerShell (méthode alternative sans interface graphique)
# Installer le module DockerMsftProvider
Install-Module -Name DockerMsftProvider -Repository PSGallery -Force


# Installer Docker
Install-Package -Name docker -ProviderName DockerMsftProvider


# Redémarrer le serveur
Restart-Computer -Force


# Vérifier l'installation
docker --version
docker info

Le Dockerfile — Construire une Image
Le Dockerfile est le fichier de configuration qui décrit comment construire votre image Docker. Exemples pour différents types d'applications :
Application Node.js
# Dockerfile — Application Node.js
FROM node:20-alpine


# Définir le répertoire de travail dans le conteneur
WORKDIR /app


# Copier les fichiers de dépendances
COPY package*.json ./


# Installer les dépendances
RUN npm ci --only=production


# Copier le code source
COPY src/ ./src/


# Exposer le port de l'application
EXPOSE 3000


# Créer un utilisateur non-root pour la sécurité
RUN addgroup -S appgroup && adduser -S appuser -G appgroup
USER appuser


# Commande de démarrage
CMD ["node", "src/app.js"]

Application Python/Flask
# Dockerfile — Application Python Flask
FROM python:3.11-slim


WORKDIR /app


# Copier et installer les dépendances
COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt


# Copier le code source
COPY . .


# Variables d'environnement
ENV FLASK_ENV=production
ENV PORT=5000


EXPOSE 5000


CMD ["gunicorn", "--bind", "0.0.0.0:5000", "app:app"]

Commandes Docker Essentielles
# Construire une image à partir du Dockerfile
docker build -t mon-app:1.0 .


# Lister les images disponibles
docker images


# Lancer un conteneur
docker run -d -p 3000:3000 --name mon-conteneur mon-app:1.0


# Voir les conteneurs en cours d'exécution
docker ps


# Voir les logs d'un conteneur
docker logs mon-conteneur


# Entrer dans un conteneur en cours d'exécution
docker exec -it mon-conteneur sh


# Arrêter et supprimer un conteneur
docker stop mon-conteneur && docker rm mon-conteneur


# Pousser une image vers Docker Hub
docker tag mon-app:1.0 moncompte/mon-app:1.0
docker push moncompte/mon-app:1.0

Docker Compose — Multi-conteneurs
Docker Compose permet de définir et gérer des applications multi-conteneurs (ex: app + base de données + cache) :
# docker-compose.yml
version: '3.8'


services:
  app:
    build: .
    ports:
      - '3000:3000'
    environment:
      - DATABASE_URL=postgresql://user:pass@db:5432/mondb
    depends_on:
      - db
      - redis


  db:
    image: postgres:15
    volumes:
      - postgres_data:/var/lib/postgresql/data
    environment:
      POSTGRES_DB: mondb
      POSTGRES_USER: user
      POSTGRES_PASSWORD: pass


  redis:
    image: redis:alpine


volumes:
  postgres_data:
# Lancer tous les services
docker-compose up -d

# Arrêter tous les services
docker-compose down

# Exemple concret pour BrasilBurger
# Le service web se connecte à la base Neon PostgreSQL distante.
# Après le build Docker, lancez l'application en conteneur :
cd projet-semestre_1_brasilburger-csharp
docker compose up -d

# Accéder à l'application
http://localhost:8083


# Voir les logs de tous les services
docker-compose logs -f

ÉTAPE 5
DEPLOY CD — Kubernetes & Déploiement Continu


Kubernetes (K8s) est un système d'orchestration de conteneurs. Il gère automatiquement le déploiement, la mise à l'échelle et la haute disponibilité de vos applications conteneurisées.

Concepts Kubernetes Fondamentaux
Objet K8s
Rôle
Pod
La plus petite unité K8s — contient un ou plusieurs conteneurs
Deployment
Gère un ensemble de Pods identiques avec rolling updates
Service
Expose un Deployment sur le réseau (IP stable, load balancing)
Namespace
Espace de noms pour isoler des ressources (dev, staging, prod)
ConfigMap
Stocke des configurations non-sensibles (variables d'env)
Secret
Stocke des données sensibles (mots de passe, clés API)
Ingress
Gère l'accès HTTP/HTTPS externe vers les services internes
Node
Machine (physique ou virtuelle) faisant partie du cluster


Installation de Kubernetes sur Windows Server
Option 1 : Activer Kubernetes via Docker Desktop
Docker Desktop inclut une option pour activer un cluster Kubernetes local en un clic : Paramètres > Kubernetes > Enable Kubernetes.

Option 2 : Installer kubectl (CLI) seul
# Télécharger kubectl via winget
winget install Kubernetes.kubectl


# Vérifier l'installation
kubectl version --client


# Configurer kubectl pour un cluster distant
# Le fichier de config est dans : C:\Users\VotreNom\.kube\config

Option 3 : Installer k3s (Kubernetes léger) sur Windows avec WSL
# Activer WSL2 (Windows Subsystem for Linux)
wsl --install


# Dans WSL Ubuntu, installer k3s
curl -sfL https://get.k3s.io | sh -


# Vérifier que le cluster fonctionne
kubectl get nodes

Déployer une Application sur Kubernetes
Fichier deployment.yaml
# kubernetes/deployment.yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: mon-app
  namespace: production
  labels:
    app: mon-app
    version: '1.0'
spec:
  replicas: 3             # 3 instances de l'application
  selector:
    matchLabels:
      app: mon-app
  strategy:
    type: RollingUpdate   # Mise à jour progressive (zéro downtime)
    rollingUpdate:
      maxSurge: 1
      maxUnavailable: 0
  template:
    metadata:
      labels:
        app: mon-app
    spec:
      containers:
      - name: mon-app
        image: mon-registry.local/mon-app:latest
        ports:
        - containerPort: 3000
        resources:
          requests:
            memory: '128Mi'
            cpu: '100m'
          limits:
            memory: '256Mi'
            cpu: '500m'
        livenessProbe:    # Vérification que l'app est vivante
          httpGet:
            path: /health
            port: 3000
          initialDelaySeconds: 30
          periodSeconds: 10

Fichier service.yaml
# kubernetes/service.yaml
apiVersion: v1
kind: Service
metadata:
  name: mon-app-service
  namespace: production
spec:
  selector:
    app: mon-app
  ports:
    - protocol: TCP
      port: 80           # Port exposé externellement
      targetPort: 3000   # Port du conteneur
  type: LoadBalancer     # Ou 'ClusterIP' pour usage interne uniquement

Commandes kubectl Essentielles
# Appliquer une configuration
kubectl apply -f kubernetes/


# Voir les Pods en cours
kubectl get pods -n production


# Voir les détails d'un Pod
kubectl describe pod <nom-du-pod> -n production


# Voir les logs d'un Pod
kubectl logs <nom-du-pod> -n production


# Mettre à jour l'image d'un Deployment (rolling update)
kubectl set image deployment/mon-app mon-app=mon-registry.local/mon-app:v2.0


# Vérifier l'avancement d'un rolling update
kubectl rollout status deployment/mon-app


# Revenir à la version précédente en cas de problème
kubectl rollout undo deployment/mon-app


# Scaler horizontalement (augmenter le nombre de réplicas)
kubectl scale deployment mon-app --replicas=5

ÉTAPE 6
MONITOR — Grafana, Prometheus & ELK


Le monitoring est la dernière étape mais l'une des plus importantes. Sans monitoring, vous ne savez pas ce qui se passe réellement en production. Les outils de monitoring surveillent les performances, détectent les anomalies et centralisent les logs.

Architecture du Monitoring
Outil
Rôle
Port par défaut
Prometheus
Collecte et stocke les métriques
9090
Grafana
Visualise les métriques (tableaux de bord)
3000
Node Exporter
Exporte les métriques système (CPU, RAM, disque)
9100
Alertmanager
Envoie des alertes (email, Slack) selon des règles
9093
Elasticsearch
Stocke et indexe les logs (stack ELK)
9200
Logstash
Collecte, transforme et envoie les logs
5044
Kibana
Interface de visualisation des logs ELK
5601


Installation de Prometheus sur Windows Server
# Télécharger Prometheus depuis https://prometheus.io/download/
# Choisir la version windows-amd64


# Extraire dans C:\prometheus
# Démarrer Prometheus
cd C:\prometheus
.\prometheus.exe --config.file=prometheus.yml


# Installer Prometheus comme service Windows
sc create Prometheus binPath= 'C:\prometheus\prometheus.exe --config.file=C:\prometheus\prometheus.yml'
sc start Prometheus


# Accès : http://VOTRE-IP:9090

Configuration Prometheus (prometheus.yml)
# prometheus.yml
global:
  scrape_interval: 15s      # Collecte les métriques toutes les 15 secondes
  evaluation_interval: 15s


# Règles d'alertes
rule_files:
  - 'alerting_rules.yml'


# Configuration de l'Alertmanager
alerting:
  alertmanagers:
    - static_configs:
        - targets: ['localhost:9093']


# Cibles à surveiller (scrape_configs)
scrape_configs:
  # Prometheus se surveille lui-même
  - job_name: 'prometheus'
    static_configs:
      - targets: ['localhost:9090']


  # Métriques système Windows
  - job_name: 'windows_server'
    static_configs:
      - targets: ['localhost:9182']   # windows_exporter


  # Votre application
  - job_name: 'mon-app'
    static_configs:
      - targets: ['localhost:3000']
    metrics_path: '/metrics'


  # Kubernetes
  - job_name: 'kubernetes-pods'
    kubernetes_sd_configs:
      - role: pod

Installation et Configuration de Grafana
# Télécharger Grafana depuis https://grafana.com/grafana/download?platform=windows
# Installer via le fichier .msi


# Grafana s'installe comme service Windows automatiquement
# Vérifier le service
Get-Service -Name Grafana


# Accès : http://VOTRE-IP:3000
# Connexion par défaut : admin / admin
# Changer le mot de passe immédiatement !

Connexion Grafana vers Prometheus
Se connecter à Grafana (http://VOTRE-IP:3000)
Aller dans : Configuration > Data Sources > Add data source
Choisir 'Prometheus'
URL : http://localhost:9090 puis cliquer 'Save & Test'

Règles d'Alertes Prometheus
# alerting_rules.yml
groups:
  - name: infrastructure
    rules:
    # Alerte si le CPU dépasse 85% pendant 5 minutes
    - alert: CPUElevee
      expr: 100 - (avg by(instance) (rate(node_cpu_seconds_total{mode='idle'}[5m])) * 100) > 85
      for: 5m
      labels:
        severity: warning
      annotations:
        summary: 'CPU élevé sur {{ $labels.instance }}'
        description: 'CPU à {{ $value }}%'


    # Alerte si la mémoire disponible est inférieure à 10%
    - alert: MemoireFaible
      expr: (node_memory_MemAvailable_bytes / node_memory_MemTotal_bytes) * 100 < 10
      for: 2m
      labels:
        severity: critical
      annotations:
        summary: 'Mémoire critique sur {{ $labels.instance }}'


    # Alerte si un service est down
    - alert: ServiceDown
      expr: up == 0
      for: 1m
      labels:
        severity: critical
      annotations:
        summary: 'Service {{ $labels.job }} est DOWN'

ELK Stack — Centralisation des Logs
La stack ELK (Elasticsearch + Logstash + Kibana) centralise tous les logs de vos applications et serveurs pour faciliter le débogage.
Lancer ELK Stack via Docker Compose
# elk-stack/docker-compose.yml
version: '3.8'
services:
  elasticsearch:
    image: docker.elastic.co/elasticsearch/elasticsearch:8.11.0
    environment:
      - discovery.type=single-node
      - xpack.security.enabled=false
      - 'ES_JAVA_OPTS=-Xms512m -Xmx512m'
    ports:
      - '9200:9200'
    volumes:
      - esdata:/usr/share/elasticsearch/data


  logstash:
    image: docker.elastic.co/logstash/logstash:8.11.0
    ports:
      - '5044:5044'
    volumes:
      - .\logstash.conf:/usr/share/logstash/pipeline/logstash.conf


  kibana:
    image: docker.elastic.co/kibana/kibana:8.11.0
    ports:
      - '5601:5601'
    environment:
      - ELASTICSEARCH_HOSTS=http://elasticsearch:9200


volumes:
  esdata:

Pipeline Complet — Résumé et Bonnes Pratiques

Vue d'Ensemble du Pipeline
Voici le flux complet d'une modification de code jusqu'à la production :
1. DÉVELOPPEUR écrit du code sur son poste (VS Code)
   → git add . && git commit -m 'feat: nouvelle fonctionnalité'
   → git push origin feature/ma-branche


2. GITHUB/GITLAB reçoit le push et notifie Jenkins (Webhook)


3. JENKINS déclenche le pipeline automatiquement :
   → Checkout du code
   → npm install (installation dépendances)
   → npm test (exécution des tests)
   → Si tests OK : docker build (construction image)
   → docker push (envoi vers registry)


4. DOCKER IMAGE est stockée dans le registry avec un tag unique
   (ex: mon-app:build-42)


5. JENKINS déclenche le déploiement Kubernetes :
   → kubectl set image deployment/mon-app mon-app=mon-app:build-42
   → Kubernetes fait un rolling update (zéro downtime)
   → 3 réplicas mis à jour un par un


6. MONITORING surveille en continu :
   → Prometheus collecte les métriques toutes les 15 secondes
   → Grafana affiche les tableaux de bord en temps réel
   → Alertmanager envoie une alerte si quelque chose se passe mal

Checklist Sécurité DevOps


Action
Outil/Méthode
✓
Ne jamais committer de secrets dans Git
.gitignore + variables d'environnement
✓
Scanner les images Docker pour les vulnérabilités
trivy, docker scout
✓
Utiliser des utilisateurs non-root dans les conteneurs
USER appuser dans le Dockerfile
✓
Chiffrer les communications avec TLS/SSL
Certificates + HTTPS
✓
Stocker les credentials Jenkins de manière sécurisée
Jenkins Credentials Manager
✓
Sauvegarder régulièrement les données et volumes
Windows Backup + snapshots
✓
Mettre à jour régulièrement les images de base Docker
Dependabot / Renovate
✓
Restreindre les accès Kubernetes avec RBAC
Roles et RoleBindings K8s


Bonnes Pratiques — Résumé
Un commit = une modification logique avec un message clair et descriptif
Les tests doivent passer à 100% avant tout déploiement en production
Toujours avoir un environnement de staging (pré-production) avant la production
Utiliser des tags de version sémantique pour les images Docker (v1.2.3)
Documenter les runbooks : que faire quand une alerte se déclenche ?
Faire des revues de code (Pull Requests) systématiquement
Automatiser tout ce qui est répétitif — rien ne doit être fait à la main en production

Annexes & Commandes Essentielles

Référence Rapide — Toutes les Commandes
Git
Commande
Action
git init
Initialiser un dépôt
git clone <url>
Cloner un dépôt
git add . && git commit -m 'msg'
Committer toutes les modifications
git push origin main
Envoyer vers le dépôt distant
git pull
Récupérer les dernières modifications
git log --oneline --graph
Voir l'historique visuel
git stash
Sauvegarder temporairement des modifications


Docker
Commande
Action
docker build -t app:tag .
Construire une image
docker run -d -p 80:3000 app:tag
Lancer un conteneur
docker ps
Lister les conteneurs actifs
docker logs <nom>
Voir les logs
docker exec -it <nom> sh
Entrer dans le conteneur
docker-compose up -d
Lancer l'ensemble des services
docker system prune
Nettoyer les ressources inutilisées


Kubernetes (kubectl)
Commande
Action
kubectl get pods
Lister les Pods
kubectl get services
Lister les Services
kubectl apply -f fichier.yaml
Appliquer une configuration
kubectl describe pod <nom>
Détails d'un Pod
kubectl logs <pod>
Logs d'un Pod
kubectl rollout undo deployment/app
Rollback version précédente
kubectl get events
Voir les événements du cluster


Ports et Services — Tableau de Référence
Service
Port
URL d'accès
Jenkins
8080
http://SERVEUR:8080
Grafana
3000
http://SERVEUR:3000
Prometheus
9090
http://SERVEUR:9090
Alertmanager
9093
http://SERVEUR:9093
Kibana (ELK)
5601
http://SERVEUR:5601
Elasticsearch
9200
http://SERVEUR:9200
Kubernetes API
6443
https://SERVEUR:6443
Docker Registry
5000
http://SERVEUR:5000


Ressources Complémentaires
Documentation officielle Docker : https://docs.docker.com
Documentation Kubernetes : https://kubernetes.io/docs
Documentation Jenkins : https://www.jenkins.io/doc
Documentation Prometheus : https://prometheus.io/docs
Documentation Grafana : https://grafana.com/docs
Git Flow : https://nvie.com/posts/a-successful-git-branching-model

À Retenir
Le DevOps n'est pas qu'une question d'outils — c'est avant tout une culture de collaboration.
Commencez petit : un pipeline simple avec Git + Jenkins + Docker apporte déjà une valeur énorme.
Automatisez progressivement : n'essayez pas de tout mettre en place en même temps.
Documentez : chaque configuration, chaque procédure doit être écrite et versionnée dans Git.
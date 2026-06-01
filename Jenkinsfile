pipeline {
    agent any

    options {
        skipDefaultCheckout()
        disableConcurrentBuilds()
    }

    environment {
        UBUNTU_IP  = '192.168.1.27'
        UBUNTU_USER = 'devops'
        APP_NAME   = 'brasilburger'
        DOCKER_TAG = "${env.BUILD_NUMBER}"
        REPO_URL   = 'https://github.com/yacine004/projet-DevOps'
    }

    stages {

        stage('Checkout') {
            steps {
                checkout([
                    $class: 'GitSCM',
                    branches: [[name: '*/main']],
                    userRemoteConfigs: [[
                        url: 'https://github.com/yacine004/projet-DevOps',
                        credentialsId: 'github-credentials'
                    ]]
                ])
            }
        }

        stage('Build .NET') {
            steps {
                bat 'cd csharp_web && dotnet restore'
                bat 'cd csharp_web && dotnet build --configuration Release --no-restore'
            }
        }

        stage('Tests') {
            steps {
                catchError(buildResult: 'SUCCESS', stageResult: 'UNSTABLE') {
                    bat 'cd csharp_web && dotnet test --no-build --configuration Release'
                }
            }
        }

        stage('Build et Push Docker (Ubuntu)') {
            steps {
                script {
                    def remote = [:]
                    remote.name        = 'ubuntu'
                    remote.host        = env.UBUNTU_IP
                    remote.user        = env.UBUNTU_USER
                    remote.allowAnyHosts = true

                    withCredentials([
                        sshUserPrivateKey(credentialsId: 'ubuntu-ssh', keyFileVariable: 'SSH_KEY'),
                        usernamePassword(credentialsId: 'docker-hub-credentials', usernameVariable: 'DOCKER_USER', passwordVariable: 'DOCKER_PASS')
                    ]) {
                        remote.identityFile = SSH_KEY

                        // Clone ou pull le repo sur Ubuntu
                        sshCommand remote: remote, command: """
                            if [ -d /home/devops/app/.git ]; then
                                cd /home/devops/app && git pull origin main
                            else
                                rm -rf /home/devops/app && git clone https://github.com/yacine004/projet-DevOps /home/devops/app
                            fi
                        """

                        // Docker build
                        sshCommand remote: remote, command: "cd /home/devops/app && docker build -t yacine1108/brasilburger:${env.DOCKER_TAG} . && docker tag yacine1108/brasilburger:${env.DOCKER_TAG} yacine1108/brasilburger:latest"

                        // Docker push
                        sshCommand remote: remote, command: "docker login -u ${DOCKER_USER} -p ${DOCKER_PASS} && docker push yacine1108/brasilburger:${env.DOCKER_TAG} && docker push yacine1108/brasilburger:latest"
                    }
                }
            }
        }

        stage('Deploy Kubernetes') {
            steps {
                script {
                    def remote = [:]
                    remote.name        = 'ubuntu'
                    remote.host        = env.UBUNTU_IP
                    remote.user        = env.UBUNTU_USER
                    remote.allowAnyHosts = true

                    withCredentials([sshUserPrivateKey(credentialsId: 'ubuntu-ssh', keyFileVariable: 'SSH_KEY')]) {
                        remote.identityFile = SSH_KEY

                        sshCommand remote: remote, command: """
                            kubectl apply -f /home/devops/app/kubernetes/ -n production
                            kubectl set image deployment/brasilburger-deployment brasilburger=yacine1108/brasilburger:${env.DOCKER_TAG} -n production
                            kubectl rollout status deployment/brasilburger-deployment -n production
                        """
                    }
                }
            }
        }
    }

    post {
        success {
            echo 'Pipeline reussi — BrasilBurger deploye avec succes !'
        }
        failure {
            echo 'Echec du pipeline — voir les logs ci-dessus'
        }
    }
}

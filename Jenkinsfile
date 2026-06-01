pipeline {
    agent any

    options {
        skipDefaultCheckout()
        disableConcurrentBuilds()
    }

    environment {
        UBUNTU_IP   = '192.168.1.27'
        UBUNTU_USER = 'devops'
        APP_NAME    = 'brasilburger'
        DOCKER_TAG  = "${env.BUILD_NUMBER}"
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

        stage('Transfert vers Ubuntu') {
            steps {
                sshagent(['ubuntu-ssh']) {
                    bat "ssh -o StrictHostKeyChecking=no %UBUNTU_USER%@%UBUNTU_IP% \"mkdir -p /home/devops/app\""
                    bat "scp -r -o StrictHostKeyChecking=no . %UBUNTU_USER%@%UBUNTU_IP%:/home/devops/app/"
                }
            }
        }

        stage('Docker Build (Ubuntu)') {
            steps {
                sshagent(['ubuntu-ssh']) {
                    bat "ssh -o StrictHostKeyChecking=no %UBUNTU_USER%@%UBUNTU_IP% \"cd /home/devops/app && docker build -t %APP_NAME%:%DOCKER_TAG% . && docker tag %APP_NAME%:%DOCKER_TAG% %APP_NAME%:latest\""
                }
            }
        }

        stage('Push Docker Hub') {
            steps {
                withCredentials([usernamePassword(credentialsId: 'docker-hub-credentials', usernameVariable: 'DOCKER_USERNAME', passwordVariable: 'DOCKER_PASSWORD')]) {
                    sshagent(['ubuntu-ssh']) {
                        bat "ssh -o StrictHostKeyChecking=no %UBUNTU_USER%@%UBUNTU_IP% \"docker login -u %DOCKER_USERNAME% -p %DOCKER_PASSWORD% && docker tag %APP_NAME%:latest yacine1108/brasilburger:%DOCKER_TAG% && docker push yacine1108/brasilburger:%DOCKER_TAG% && docker push yacine1108/brasilburger:latest\""
                    }
                }
            }
        }

        stage('Deploy Kubernetes') {
            when { branch 'main' }
            steps {
                sshagent(['ubuntu-ssh']) {
                    bat "ssh -o StrictHostKeyChecking=no %UBUNTU_USER%@%UBUNTU_IP% \"kubectl apply -f /home/devops/app/kubernetes/ && kubectl set image deployment/brasilburger-deployment brasilburger=%APP_NAME%:%DOCKER_TAG% -n production && kubectl rollout status deployment/brasilburger-deployment -n production\""
                }
            }
        }
    }

    post {
        success {
            echo 'Pipeline réussi — BrasilBurger déployé avec succès !'
        }
        failure {
            echo 'Échec du pipeline — voir les logs ci-dessus'
        }
    }
}
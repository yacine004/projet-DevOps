pipeline {
    agent any

    options {
        skipDefaultCheckout()
        disableConcurrentBuilds()
    }

    stages {

        stage('Checkout') {
            steps {
                checkout([
                    $class: 'GitSCM',
                    branches: [[name: '*/main']],
                    userRemoteConfigs: [[url: 'https://github.com/yacine004/projet-DevOps.git']]
                ])
            }
        }

        stage('Build .NET') {
            steps {
                bat 'cd csharp_web && dotnet build --configuration Release'
            }
        }

        stage('Build Docker') {
            steps {
                bat 'docker build -t brasilburger .'
            }
        }

        stage('Run Container') {
            steps {
                bat 'docker stop brasilburger_ci || exit 0'
                bat 'docker rm brasilburger_ci || exit 0'
                bat 'docker run -d --name brasilburger_ci -p 8084:8080 brasilburger'
            }
        }
    }
}
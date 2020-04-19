#!/bin/bash

set -x

if [ -f /etc/debian_version ]; then
    apt-get update
    apt-get upgrade -y
    apt-get install -y apt-transport-https docker.io

    curl -s https://packages.cloud.google.com/apt/doc/apt-key.gpg | apt-key add -
    cat <<EOF >/etc/apt/sources.list.d/kubernetes.list
deb http://apt.kubernetes.io/ kubernetes-xenial main
EOF

    apt-get update
    apt-get install -y kubelet kubeadm kubectl
fi

if [ -f /etc/redhat-release ]; then
    yum upgrade -y
    yum install -y docker
    systemctl enable docker
    systemctl start docker

    cat <<EOF >  /etc/sysctl.d/k8s.conf
net.bridge.bridge-nf-call-ip6tables = 1
net.bridge.bridge-nf-call-iptables = 1
EOF
    sysctl --system

    cat <<EOF > /etc/yum.repos.d/kubernetes.repo
[kubernetes]
name=Kubernetes
baseurl=https://packages.cloud.google.com/yum/repos/kubernetes-el7-x86_64
enabled=1
gpgcheck=1
repo_gpgcheck=1
gpgkey=https://packages.cloud.google.com/yum/doc/yum-key.gpg https://packages.cloud.google.com/yum/doc/rpm-package-key.gpg
EOF

    setenforce 0
    yum install -y kubelet kubeadm kubectl
    systemctl enable kubelet
    systemctl start kubelet
fi

kubeadm init --pod-network-cidr=10.244.0.0/16

kubectl --kubeconfig=/etc/kubernetes/admin.conf apply -f https://raw.githubusercontent.com/coreos/flannel/v0.9.1/Documentation/kube-flannel.yml

kubectl --kubeconfig=/etc/kubernetes/admin.conf taint nodes --all node-role.kubernetes.io/master-kubectl taint nodes --all node-role.kubernetes.io/master-

install -o 1000 -d /home/$(id -nu 1000)/.kube
install -o 1000 /etc/kubernetes/admin.conf /home/$(id -nu 1000)/.kube/config
